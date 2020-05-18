﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BlazorWebDavFunctionsApp.Models;
using Newtonsoft.Json;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace Blazor.WebDAV.AudioPlayer.Client
{
    public class WebDavClient: IWebDavClient
    {
        private readonly IHttpClientFactory _factory;

        public WebDavClient(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<ResourceLoadStatus> FetchChildResourcesAsync(ResourceItem resourceItem, CancellationToken cancellationToken, int maxLevel, int level = 0)
        {
            var client = _factory.CreateClient();

            var content = await client.GetStringAsync("api/FetchChildResources");

            var result = JsonConvert.DeserializeObject<FetchChildResourcesResult>(content);

            //resourceItem = result.ResourceItem;

            return result.ResourceLoadStatus;
        }

        public Task<ResourceLoadStatus> GetStreamAsync(ResourceItem resourceItem, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResourceLoadStatus> DownloadFolderAsync(ResourceItem folder, string destinationFolder, Action<bool, ResourceItem, int, int> notify, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}