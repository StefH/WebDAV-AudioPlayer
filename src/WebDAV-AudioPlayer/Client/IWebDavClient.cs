using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebDav.AudioPlayer.Models;

namespace WebDav.AudioPlayer.Client
{
    interface IWebDavClient : IDisposable
    {
        Task<List<ResourceItem>> ListResourcesAsync(Uri path, CancellationToken cancellationToken);

        Task<ResourceLoadStatus> GetStreamAsync(ResourceItem resourceItem, CancellationToken cancellationToken);
    }
}