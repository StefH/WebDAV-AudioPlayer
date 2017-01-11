using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static SemaphoreSlim semaphore = new SemaphoreSlim(5, 5);

        private static Func<WebDavResource, bool> _isAudioFile = r => r.Uri.EndsWith(".wav") || r.Uri.EndsWith(".wma") || r.Uri.EndsWith(".mp3") || r.Uri.EndsWith(".mp4") || r.Uri.EndsWith(".m4a") || r.Uri.EndsWith(".aac") || r.Uri.EndsWith(".ogg") || r.Uri.EndsWith(".flac");
        private static Func<WebDavResource, bool> _isFolder = r => r.IsCollection;

        private readonly WebDavClient _client;
        private readonly IConnectionSettings _connectionSettings;

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

        public async Task<List<ResourceItem>> ListResourcesAsync(Uri path, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            Debug.WriteLine("path=[" + path + "]");

            if (path == null)
                path = OnlinePathBuilder.ConvertPathToFullUri(_connectionSettings.StorageUri, _connectionSettings.RootFolder);

            var result = await _client.Propfind(path, new PropfindParameters { CancellationToken = cancellationToken });
            if (result.Resources != null)
            {
                var items = result.Resources.Skip(1)
                    .Where(r => _isAudioFile(r) || _isFolder(r))
                    .Select(r =>
                    {
                        Uri fullPath = OnlinePathBuilder.Combine(_connectionSettings.StorageUri, r.Uri);
                        var resourceItem = new ResourceItem
                        {
                            DisplayName = r.DisplayName,
                            IsCollection = r.IsCollection,
                            FullPath = fullPath,
                            ContentLength = r.ContentLength
                        };

                        return resourceItem;
                    });

                return items.OrderBy(r => r.DisplayName).ToList();
            }

            return null;
        }

        public async Task<List<ResourceItem>> ListResourcesAsyncX(Uri path, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            Debug.WriteLine("path=[" + path + "]");

            if (path == null)
                path = OnlinePathBuilder.ConvertPathToFullUri(_connectionSettings.StorageUri, _connectionSettings.RootFolder);

            PropfindResponse result;
            try
            {
                Debug.WriteLine("semaphore.CurrentCount = [" + semaphore.CurrentCount + "]");
                await semaphore.WaitAsync(cancellationToken);

                result = await _client.Propfind(path, new PropfindParameters { CancellationToken = cancellationToken });
            }
            finally
            {
                semaphore.Release();
            }

            if (result != null && result.Resources != null)
            {
                var tasks = result.Resources.Skip(1)
                    .Where(r => _isAudioFile(r) || _isFolder(r))
                    .Select(async r =>
                    {
                        Uri fullPath = OnlinePathBuilder.Combine(_connectionSettings.StorageUri, r.Uri);
                        var resourceItem = new ResourceItem
                        {
                            DisplayName = r.DisplayName,
                            IsCollection = r.IsCollection,
                            FullPath = fullPath,
                            ContentLength = r.ContentLength
                        };

                        if (r.IsCollection)
                        {
                            //resourceItem.Items = await ListResourcesAsyncX(fullPath, cancellationToken);
                        }

                        return resourceItem;
                    }).ToArray();

                await Task.WhenAll(tasks);
            }

            return null;
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
                var webDavStreamResponse = await _client.GetRawFile(resourceItem.FullPath).WithCancellation(cancellationToken);
                if (webDavStreamResponse.IsSuccessful)
                {
                    resourceItem.Stream = webDavStreamResponse.Stream;

                    resourceItem.MediaDetails = MediaInfoHelper.GetMediaDetails(resourceItem.Stream);

                    return ResourceLoadStatus.StreamLoaded;
                }

                resourceItem.Stream = null;
                return ResourceLoadStatus.StreamFailedToLoad;
            }
            catch (OperationCanceledException)
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