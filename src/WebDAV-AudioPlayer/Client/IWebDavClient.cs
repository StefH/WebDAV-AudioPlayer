using System;
using System.Threading;
using System.Threading.Tasks;
using WebDav.AudioPlayer.Models;

namespace WebDav.AudioPlayer.Client
{
    public interface IWebDavClient : IDisposable
    {
        Task<ResourceLoadStatus> FetchChildResourcesAsync(ResourceItem resourceItem, CancellationToken cancellationToken, int maxLevel, int level = 0);

        Task<ResourceLoadStatus> GetStreamAsync(ResourceItem resourceItem, CancellationToken cancellationToken);

        Task<ResourceLoadStatus> DownloadFolderAsync(ResourceItem folder, string destinationFolder, Action<bool, ResourceItem, int, int> notify, CancellationToken cancellationToken);
    }
}