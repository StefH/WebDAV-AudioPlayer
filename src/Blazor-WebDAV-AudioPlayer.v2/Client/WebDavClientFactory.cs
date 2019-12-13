using Blazor.WebDAV.AudioPlayer.Options;
using Microsoft.Extensions.Options;
using System;
using WebDav.AudioPlayer.Client;

namespace Blazor.WebDAV.AudioPlayer.Client
{
    public class WebDavClientFactory : IWebDavClientFactory
    {
        private readonly IOptions<ConnectionSettings> _options;

        public WebDavClientFactory(IOptions<ConnectionSettings> options)
        {
            _options = options;
        }

        public IWebDavClient GetClient(string[] audioExtensions)
        {
            var con = new ConnectionSettings
            {
                Password = _options.Value.Password,
                UserName = _options.Value.UserName,
                RootFolder = _options.Value.RootFolder,
                StorageUri = _options.Value.StorageUri
            };
            return new MyWebDavClient(con, audioExtensions);
        }
    }
}
