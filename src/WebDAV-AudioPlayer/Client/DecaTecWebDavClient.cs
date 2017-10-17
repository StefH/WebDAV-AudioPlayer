using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DecaTec.WebDav;
using DecaTec.WebDav.WebDavArtifacts;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.Models;

namespace WebDav.AudioPlayer.Client
{
    public class DecaTecWebDavClient : IWebDavClient
    {
        private static readonly string[] AudioExtensions = { ".wav", ".wma", ".mp3", ".mp4", ".m4a", ".aac", ".ogg", ".flac" };
        private static readonly Func<WebDavSessionListItem, bool> IsAudioFile = r => AudioExtensions.Any(e => r.Name.ToLowerInvariant().EndsWith(e));
        private static readonly Func<WebDavSessionListItem, bool> IsFolder = r => r.IsCollection;

        private readonly WebDavSession _session;
        private readonly IConnectionSettings _connectionSettings;

        public DecaTecWebDavClient(IConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;

            var credentials = new NetworkCredential(_connectionSettings.UserName, _connectionSettings.GetPassword());

            _session = new WebDavSession(credentials);
        }

        public async Task<ResourceLoadStatus> FetchChildResourcesAsync(ResourceItem parent, CancellationToken cancellationToken, int maxLevel, int level = 0)
        {
            if (cancellationToken.IsCancellationRequested)
                return ResourceLoadStatus.OperationCanceled;

            if (level > maxLevel)
                return ResourceLoadStatus.OperationCanceled;

            var propfind = PropFind.CreatePropFindWithEmptyProperties(
                PropNameConstants.Name,
                PropNameConstants.DisplayName,
                PropNameConstants.IsCollection,
                PropNameConstants.ResourceType,
                PropNameConstants.GetContentLength
            );

            Debug.WriteLine("path=[" + parent.FullPath + "], level = [" + level + "] " + "ListAsync start : " + DateTime.UtcNow);
            IList<WebDavSessionListItem> result;
            try
            {
                result = await _session.ListAsync(parent.FullPath, propfind); // .TimeoutAfter(TimeSpan.FromSeconds(1), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return ResourceLoadStatus.OperationCanceled;
            }
            catch (TimeoutException)
            {
                return ResourceLoadStatus.OperationCanceled;
            }
            Debug.WriteLine("path=[" + parent.FullPath + "], level = [" + level + "] " + "ListAsync   end : " + DateTime.UtcNow);

            if (result != null)
            {
                var tasks = result
                    .Where(r => IsAudioFile(r) || IsFolder(r))
                    .Select(async r =>
                    {
                        //Debug.WriteLine("WebDavSessionListItem = " + JsonConvert.SerializeObject(r, Formatting.Indented));

                        var resourceItem = new ResourceItem
                        {
                            Level = level,
                            DisplayName = r.DisplayName,
                            IsCollection = r.IsCollection,
                            FullPath = r.Uri,
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
                return ResourceLoadStatus.OperationCanceled;

            if (resourceItem.IsCollection)
                return ResourceLoadStatus.IsCollection;

            if (resourceItem.Stream != null && resourceItem.Stream.CanRead)
                return ResourceLoadStatus.StreamExisting;

            try
            {
                resourceItem.Stream = new MemoryStream();
                bool isSuccessful = await _session.DownloadFileAsync(resourceItem.FullPath, resourceItem.Stream, cancellationToken);
                if (isSuccessful)
                {
                    resourceItem.MediaDetails = MediaInfoHelper.GetMediaDetails(resourceItem.Stream);

                    return ResourceLoadStatus.Ok;
                }

                resourceItem.Stream.Close();
                resourceItem.Stream.Dispose();
                resourceItem.Stream = null;

                return ResourceLoadStatus.StreamFailedToLoad;
            }
            catch (TaskCanceledException)
            {
                return ResourceLoadStatus.OperationCanceled;
            }
        }

        public async Task<ResourceLoadStatus> DownloadFolderAsync(ResourceItem folder, string destinationFolder, Action<bool, ResourceItem, int, int> notify, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return ResourceLoadStatus.OperationCanceled;

            try
            {
                if (folder.Items == null)
                {
                    var fetchResult = await FetchChildResourcesAsync(folder, cancellationToken, folder.Level + 1, folder.Level);
                    if (fetchResult != ResourceLoadStatus.Ok)
                        return fetchResult;
                }

                var files = folder.Items.Where(i => !i.IsCollection).ToArray();
                if (!files.Any())
                    return ResourceLoadStatus.NoResourcesFound;

                string folderPath = Path.Combine(destinationFolder, folder.DisplayName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var tasks = new List<Task>();
                for (int i = 0; i < files.Length; i++)
                {
                    ResourceItem fileResource = files[i];

                    string filePath = Path.Combine(folderPath, fileResource.DisplayName);
                    var fileStream = new FileStream(filePath, FileMode.Create);

                    var task = _session.DownloadFileAsync(fileResource.FullPath, fileStream, cancellationToken);
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks.ToArray());

                //int index = 0;
                //foreach (var fileResource in files)
                //{
                //    string filePath = Path.Combine(folderPath, fileResource.DisplayName);
                //    var fileStream = new FileStream(filePath, FileMode.Create);

                //    bool result = await _session.DownloadFileAsync(fileResource.FullPath, fileStream, cancellationToken);
                //    fileStream.Flush();

                //    //notify(result, fileResource, index++, files.Length);
                //}

                //var result = await Task.WhenAll(tasks.ToArray());

                //for (int i = 0; i < files.Length; i++)
                //{
                //    ResourceItem fileResource = files[i];

                //    string filePath = Path.Combine(folderPath, fileResource.DisplayName);
                //    var fileStream = new FileStream(filePath, FileMode.Create);

                //    bool isSuccessful = await _session.DownloadFileAsync(fileResource.FullPath, fileStream, cancellationToken);

                //    notify(isSuccessful, fileResource, i, files.Length);
                //}

                return ResourceLoadStatus.Ok;
            }
            catch (TaskCanceledException)
            {
                return ResourceLoadStatus.OperationCanceled;
            }
        }

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
