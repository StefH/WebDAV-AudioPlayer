using Blazor.WebDAV.AudioPlayer.Options;
using Microsoft.Extensions.Options;
using WebDav.AudioPlayer.Client;

namespace Blazor.WebDAV.AudioPlayer.Client;

public class WebDavClientFactory(IOptions<ConnectionSettings> options) : IWebDavClientFactory
{
    public IWebDavClient GetClient()
    {
        var con = new ConnectionSettings
        {
            Password = options.Value.Password,
            UserName = options.Value.UserName,
            RootFolder = options.Value.RootFolder,
            StorageUri = options.Value.StorageUri
        };
        return new AudioPlayerWebDavClient(con);
    }
}