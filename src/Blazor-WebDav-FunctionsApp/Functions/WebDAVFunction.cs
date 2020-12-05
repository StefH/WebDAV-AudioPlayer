using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlazorWebDavFunctionsApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebDav;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace BlazorWebDavFunctionsApp.Functions
{
    public sealed class WebDAVFunction
    {
        private readonly ILogger<WebDAVFunction> _logger;
        private readonly IConnectionSettings _connectionSettings;
        private readonly WebDavClient _client;

        public WebDAVFunction(ILogger<WebDAVFunction> logger, IConnectionSettings connectionSettings)
        {
            _logger = logger;

            _connectionSettings = connectionSettings;

            _client = new WebDavClient(new WebDavClientParams
            {
                Credentials = new NetworkCredential(_connectionSettings.UserName, _connectionSettings.GetPassword()),
                UseDefaultCredentials = false,
                Timeout = TimeSpan.FromMinutes(5)
            });
        }

        [FunctionName("GetRoot")]
        public ResourceItem GetRoot([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            return new ResourceItem
            {
                DisplayName = _connectionSettings.RootFolder,
                FullPath = OnlinePathBuilder.Combine(_connectionSettings.StorageUri, _connectionSettings.RootFolder)
            };
        }


        [FunctionName("FetchChildResources")]
        public async Task<FetchChildResourcesResult> FetchChildResources([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("{method} executed at: {now}", DateTime.Now, nameof(FetchChildResources));

            string body = await req.ReadAsStringAsync();
            var resourceItem = JsonConvert.DeserializeObject<ResourceItem>(body);

            var result = await FetchChildResourcesAsync(resourceItem, resourceItem.Level, resourceItem.Level);
            return new FetchChildResourcesResult
            {
                ResourceItem = resourceItem,
                ResourceLoadStatus = result
            };

            //var result = await _client.Propfind(parent.FullPath);
            //if (result.StatusCode == (int)HttpStatusCode.Unauthorized)
            //{
            //    throw new UnauthorizedAccessException();
            //}

            //if (result.Resources != null)
            //{
            //    var tasks = result.Resources.Skip(1)
            //        .Where(r => !r.DisplayName.StartsWith("."))
            //        .Select(async r =>
            //        {
            //            Uri fullPath = OnlinePathBuilder.Combine(_connectionSettings.StorageUri, r.Uri);
            //            var resourceItem = new ResourceItem
            //            {
            //                Id = Guid.NewGuid(),
            //                DisplayName = r.DisplayName,
            //                Extension = new FileInfo(r.DisplayName).Extension.ToLowerInvariant(),
            //                IsCollection = r.IsCollection,
            //                FullPath = fullPath,
            //                ContentLength = r.ContentLength,
            //                Parent = parent,
            //                ParentFullPath = parent.ParentFullPath
            //            };

            //            //if (r.IsCollection && level < maxLevel)
            //            //{
            //            //    await FetchChildResourcesAsync(resourceItem, cancellationToken, maxLevel, level + 1);
            //            //}

            //            return resourceItem;
            //        });

            //    var items = await Task.WhenAll(tasks);

            //    parent.Items = items.OrderBy(r => r.DisplayName).ToList();
            //}

            //return parent;
        }

        private async Task<ResourceLoadStatus> FetchChildResourcesAsync(ResourceItem parent, int maxLevel, int level)
        {
            var result = await _client.Propfind(parent.FullPath);
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
                            Parent = parent,
                            ParentFullPath = parent.ParentFullPath,
                            // todo : level???
                        };

                        if (r.IsCollection && level < maxLevel)
                        {
                            await FetchChildResourcesAsync(resourceItem, maxLevel, level + 1);
                        }

                        return resourceItem;
                    });

                var items = await Task.WhenAll(tasks);

                parent.Items = items.OrderBy(r => r.DisplayName).ToList();
            }

            return ResourceLoadStatus.Ok;
        }

        [FunctionName("GetStream")]
        public async Task<Stream> GetStream([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("{method} executed at: {now}", DateTime.Now, nameof(GetStream));

            string fullPath = req.GetQueryParameterDictionary()["fullPath"];

            var webDavStreamResponse = await _client.GetRawFile(fullPath);
            if (webDavStreamResponse.IsSuccessful)
            {
                return webDavStreamResponse.Stream;
            }

            return null;
        }

        //[FunctionName("GetData")]
        //public async Task<Stream> GetData([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        //{
        //    _logger.LogInformation("{method} executed at: {now}", DateTime.Now, nameof(GetData));

        //    string body = await req.ReadAsStringAsync();
        //    var resourceItem = JsonConvert.DeserializeObject<ResourceItem>(body);

        //    var resultX = new GetDataResult
        //    {
        //        ResourceLoadStatus = ResourceLoadStatus.StreamFailedToLoad,
        //        ResourceItem = resourceItem
        //    };

        //    if (resourceItem.IsCollection)
        //    {
        //        //result.ResourceLoadStatus = ResourceLoadStatus.IsCollection;
        //        return null; //result;
        //    }

        //    var webDavStreamResponse = await _client.GetRawFile(resourceItem.FullPath);
        //    if (webDavStreamResponse.IsSuccessful)
        //    {
        //        resourceItem.Stream = webDavStreamResponse.Stream;
        //        //resourceItem.MediaDetails = TrackInfoHelper.GetMediaDetails(resourceItem.Stream, new FileInfo(resourceItem.DisplayName).Extension.ToLowerInvariant());

        //        //result.ResourceLoadStatus = ResourceLoadStatus.Ok;
        //        //result.Content = ReadAllBytes(resourceItem.Stream);

        //        return resourceItem.Stream;
        //    }
        //    return null; // result
        //}

        private static byte[] ReadAllBytes(Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }

            using var newMemoryStream = new MemoryStream();
            stream.CopyTo(newMemoryStream);
            return newMemoryStream.ToArray();
        }
    }
}