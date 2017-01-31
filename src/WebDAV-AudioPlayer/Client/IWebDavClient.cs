using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebDav.AudioPlayer.Models;

namespace WebDav.AudioPlayer.Client
{
    interface IWebDavClient : IDisposable
    {
        Task<List<ResourceItem>> ListResourcesAsync(Uri path, CancellationToken cancellationToken, int maxLevel, int level = 0);

        Task<ResourceLoadStatus> GetStreamAsync(ResourceItem resourceItem, CancellationToken cancellationToken);
    }
}