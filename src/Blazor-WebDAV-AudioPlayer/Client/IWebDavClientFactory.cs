using WebDav.AudioPlayer.Client;

namespace Blazor.WebDAV.AudioPlayer.Client
{
    public interface IWebDavClientFactory
    {
        IWebDavClient GetClient();
    }
}