using System.IO;
using System.Threading.Tasks;
using RestEase;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace Blazor.WebDAV.AudioPlayer.Client
{
    [BasePath("api")]
    public interface IWebDAVFunctionApi
    {
        [Get("GetRoot")]
        Task<ResourceItem> GetRootAsync();


        [Post("FetchChildResources")]
        Task<ResourceLoadStatus> FetchChildResourcesAsync([Body] ResourceItem resourceItem);

        [Post("GetStream")]
        public Task<Stream> GetStreamAsync([Body] ResourceItem resourceItem);
    }
}