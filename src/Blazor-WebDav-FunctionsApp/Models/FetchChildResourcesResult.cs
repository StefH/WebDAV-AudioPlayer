using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace BlazorWebDavFunctionsApp.Models
{
    public class FetchChildResourcesResult
    {
        public ResourceItem ResourceItem { get; set; }

        public ResourceLoadStatus ResourceLoadStatus { get; set; }
    }
}
