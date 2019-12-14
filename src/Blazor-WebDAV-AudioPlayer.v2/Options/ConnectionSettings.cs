using Microsoft.Extensions.Options;
using System;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.IO;

namespace Blazor.WebDAV.AudioPlayer.Options
{
    public class ConnectionSettings : IConnectionSettings
    {
        //private readonly IOptions<ConnectionSettings> _options;

        //public ConnectionSettings(IOptions<ConnectionSettings> options)
        //{
        //    _options = options;
        //}

        public Uri StorageUri { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string RootFolder { get; set; }

        public string GetPassword()
        {
            return SecurePasswordHelper.Decrypt(Password);
        }
    }
}