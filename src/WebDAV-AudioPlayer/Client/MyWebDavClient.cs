using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.Extensions;
using WebDav.AudioPlayer.Models;

namespace WebDav.AudioPlayer.Client
{
    public class MyWebDavClient : IWebDavClient
    {
        private readonly WebDavClient _client;
        private readonly IConnectionSettings _connectionSettings;
        private readonly string[] _audioExtensions;

        public MyWebDavClient(IConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;

            _client = new WebDavClient(new WebDavClientParams
            {
                Credentials = new NetworkCredential(_connectionSettings.UserName, _connectionSettings.GetPassword()),
                UseDefaultCredentials = false,
                Timeout = TimeSpan.FromMinutes(5)
            });
        }

        public async Task<ResourceLoadStatus> FetchChildResourcesAsync(ResourceItem parent, CancellationToken cancellationToken, int maxLevel, int level)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ResourceLoadStatus.OperationCanceled;
            }

            if (level > maxLevel)
            {
                return ResourceLoadStatus.OperationCanceled;
            }

            Debug.WriteLine("path=[" + parent.FullPath + "]");

            var result = await _client.Propfind(parent.FullPath, new PropfindParameters { CancellationToken = cancellationToken });
            if (result.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                return ResourceLoadStatus.Unauthorized;
            }

            if (result.Resources != null)
            {
                var tasks = result.Resources.Skip(1)
                    .Where(r => !r.DisplayName.StartsWith("."))
                    .Select(async r =>
                    {
                        Uri fullPath = OnlinePathBuilder.Combine(_connectionSettings.StorageUri, r.Uri);
                        var resourceItem = new ResourceItem
                        {
                            Id = Guid.NewGuid(),
                            DisplayName = r.DisplayName,
                            Extension = new FileInfo(r.DisplayName).Extension.ToLowerInvariant(),
                            IsCollection = r.IsCollection,
                            FullPath = fullPath,
                            ContentLength = r.ContentLength,
                            Parent = parent
                        };

                        if (r.IsCollection && level < maxLevel)
                        {
                            await FetchChildResourcesAsync(resourceItem, cancellationToken, maxLevel, level + 1);
                        }

                        return resourceItem;
                    });

                var items = await Task.WhenAll(tasks);

                parent.Items = items.OrderBy(r => r.DisplayName).ToList();

                return ResourceLoadStatus.Ok;
            }

            return ResourceLoadStatus.NoResourcesFound;
        }

        public async Task<ResourceLoadStatus> GetStreamAsync(ResourceItem resourceItem, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ResourceLoadStatus.OperationCanceled;
            }

            if (resourceItem.IsCollection)
            {
                return ResourceLoadStatus.IsCollection;
            }

            if (resourceItem.Stream != null && resourceItem.Stream.CanRead)
            {
                return ResourceLoadStatus.StreamExisting;
            }

            try
            {
                var webDavStreamResponse = await _client.GetRawFile(resourceItem.FullPath).WithCancellation(cancellationToken);
                if (webDavStreamResponse.IsSuccessful)
                {
                    resourceItem.Stream = webDavStreamResponse.Stream;

                    resourceItem.MediaDetails = TrackInfoHelper.GetMediaDetails(resourceItem.Stream, new FileInfo(resourceItem.DisplayName).Extension.ToLowerInvariant());

                    return ResourceLoadStatus.Ok;
                }

                resourceItem.Stream = null;
                return ResourceLoadStatus.StreamFailedToLoad;
            }
            catch (OperationCanceledException)
            {
                return ResourceLoadStatus.OperationCanceled;
            }
        }

        public async Task<ResourceLoadStatus> DownloadFolderAsync(ResourceItem folder, string destinationFolder, Action<bool, ResourceItem, int, int> notify, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ResourceLoadStatus.OperationCanceled;
            }

            try
            {
                if (folder.Items == null)
                {
                    var fetchResult = await FetchChildResourcesAsync(folder, cancellationToken, folder.Level + 1, folder.Level);
                    if (fetchResult != ResourceLoadStatus.Ok)
                    {
                        return fetchResult;
                    }
                }

                if (!folder.Items.Any(i => !i.IsCollection))
                {
                    return ResourceLoadStatus.NoResourcesFound;
                }

                string folderPath = Path.Combine(destinationFolder, folder.DisplayName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                int idx = 0;
                foreach (var resourceItem in folder.Items)
                {
                    try
                    {
                        var status = await GetStreamAsync(resourceItem, cancellationToken);
                        bool isSuccessful = status == ResourceLoadStatus.StreamExisting || status == ResourceLoadStatus.Ok;

                        if (isSuccessful)
                        {
                            string filePath = Path.Combine(folderPath, resourceItem.DisplayName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            {
                                await resourceItem.Stream.CopyToAsync(fileStream);
                            }
                        }

                        notify(isSuccessful, resourceItem, idx, folder.Items.Count);
                        idx++;
                    }
                    finally
                    {
                        resourceItem.Stream?.Dispose();
                        resourceItem.Stream = null;
                    }
                }

                return ResourceLoadStatus.Ok;
            }
            catch (TaskCanceledException)
            {
                return ResourceLoadStatus.OperationCanceled;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}