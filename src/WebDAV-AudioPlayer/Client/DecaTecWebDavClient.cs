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
using WebDav.AudioPlayer.Extensions;
using WebDav.AudioPlayer.Models;

namespace WebDav.AudioPlayer.Client
{
    public class DecaTecWebDavClient : IWebDavClient
    {
        private static Func<WebDavSessionListItem, bool> _isAudioFile = r => r.Name.EndsWith(".wav") || r.Name.EndsWith(".wma") || r.Name.EndsWith(".mp3") || r.Name.EndsWith(".mp4") || r.Name.EndsWith(".m4a") || r.Name.EndsWith(".aac") || r.Name.EndsWith(".ogg") || r.Name.EndsWith(".flac");
        private static Func<WebDavSessionListItem, bool> _isFolder = r => r.IsCollection;

        private readonly WebDavSession _session;
        private readonly IConnectionSettings _connectionSettings;

        public DecaTecWebDavClient(IConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;

            var credentials = new NetworkCredential(_connectionSettings.UserName, _connectionSettings.GetPassword());

            _session = new WebDavSession(credentials);
        }

        public async Task<List<ResourceItem>> ListResourcesAsync(ResourceItem parent, CancellationToken cancellationToken, int maxLevel, int level = 0)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            if (level > maxLevel)
                return null;

            Uri path = parent == null
                ? OnlinePathBuilder.ConvertPathToFullUri(_connectionSettings.StorageUri, _connectionSettings.RootFolder)
                : parent.FullPath;

            var propfind = PropFind.CreatePropFindWithEmptyProperties(
                PropNameConstants.Name,
                PropNameConstants.DisplayName,
                PropNameConstants.IsCollection,
                PropNameConstants.ResourceType,
                PropNameConstants.GetContentLength
            );

            Debug.WriteLine("path=[" + path + "], level = [" + level + "] " + "ListAsync start : " + DateTime.UtcNow);
            IList<WebDavSessionListItem> result;
            try
            {
                result = await _session.ListAsync(path, propfind); // .TimeoutAfter(TimeSpan.FromSeconds(1), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (TimeoutException)
            {
                return null;
            }
            Debug.WriteLine("path=[" + path + "], level = [" + level + "] " + "ListAsync   end : " + DateTime.UtcNow);

            if (result != null)
            {
                var tasks = result
                    .Where(r => _isAudioFile(r) || _isFolder(r))
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
                            resourceItem.Items = await ListResourcesAsync(resourceItem, cancellationToken, maxLevel, level + 1);
                        }

                        return resourceItem;
                    });

                var items = await Task.WhenAll(tasks);

                return items.OrderBy(r => r.DisplayName).ToList();
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
                resourceItem.Stream = new MemoryStream();
                bool isSuccessful = await _session.DownloadFileAsync(resourceItem.FullPath, resourceItem.Stream, cancellationToken);
                if (isSuccessful)
                {
                    resourceItem.MediaDetails = MediaInfoHelper.GetMediaDetails(resourceItem.Stream);

                    return ResourceLoadStatus.StreamLoaded;
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
                    folder.Items = await ListResourcesAsync(folder, cancellationToken, folder.Level + 1, folder.Level);

                var files = folder.Items.Where(i => !i.IsCollection).ToArray();
                if (!files.Any())
                    return ResourceLoadStatus.NoFilesFoundInFolder;

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

                return ResourceLoadStatus.FolderDownloaded;
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
