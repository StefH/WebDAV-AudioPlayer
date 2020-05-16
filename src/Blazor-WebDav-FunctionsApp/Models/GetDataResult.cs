using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace BlazorWebDavFunctionsApp.Models
{
    public class GetDataResult
    {
        public ResourceItem ResourceItem { get; set; }

        public byte[] Content { get; set; }

        public ResourceLoadStatus ResourceLoadStatus { get; set; }
    }
}