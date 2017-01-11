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
using Newtonsoft.Json;
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

        public async Task<List<ResourceItem>> ListResourcesAsync(Uri path, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            Debug.WriteLine("path=[" + path + "]");

            if (path == null)
                path = OnlinePathBuilder.ConvertPathToFullUri(_connectionSettings.StorageUri, _connectionSettings.RootFolder);

            var propfind = PropFind.CreatePropFindWithEmptyProperties(
                PropNameConstants.Name,
                PropNameConstants.DisplayName,
                PropNameConstants.IsCollection,
                PropNameConstants.ResourceType,
                PropNameConstants.GetContentLength
            );

            IList<WebDavSessionListItem> result;
            try
            {
                result = await _session.ListAsync(path, propfind).WithCancellation(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }

            if (result != null)
            {
                var items = result
                    .Where(r => _isAudioFile(r) || _isFolder(r))
                    .Select(r =>
                    {
                        Debug.WriteLine("WebDavSessionListItem = " + JsonConvert.SerializeObject(r, Formatting.Indented));

                        var resourceItem = new ResourceItem
                        {
                            DisplayName = r.DisplayName,
                            IsCollection = r.IsCollection,
                            FullPath = r.Uri,
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

            var propfind = PropFind.CreatePropFindWithEmptyProperties(
                PropNameConstants.Name,
                PropNameConstants.DisplayName,
                PropNameConstants.IsCollection,
                PropNameConstants.ResourceType,
                PropNameConstants.GetContentLength
            );

            IList<WebDavSessionListItem> result;
            try
            {
                result = await _session.ListAsync(path, propfind).WithCancellation(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }

            if (result != null)
            {
                var tasks = result
                    .Where(r => _isAudioFile(r) || _isFolder(r))
                    .Select(async r =>
                    {
                        //Debug.WriteLine("WebDavSessionListItem = " + JsonConvert.SerializeObject(r, Formatting.Indented));

                        Uri fullPath = new Uri(string.Join("/", r.Uri.ToString().Split('/').Distinct()));
                        var resourceItem = new ResourceItem
                        {
                            DisplayName = r.DisplayName,
                            IsCollection = r.IsCollection,
                            FullPath = fullPath,
                            ContentLength = r.ContentLength
                        };

                        if (r.IsCollection)
                        {
                            resourceItem.Items = await ListResourcesAsyncX(fullPath, cancellationToken);
                        }

                        return resourceItem;
                    });

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
                resourceItem.Stream = new MemoryStream();
                bool isSuccessful = await _session.DownloadFileAsync(resourceItem.FullPath, resourceItem.Stream).WithCancellation(cancellationToken);
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
            catch (OperationCanceledException)
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