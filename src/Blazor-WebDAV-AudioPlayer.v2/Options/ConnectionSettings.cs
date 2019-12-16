using System;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.IO;

namespace Blazor.WebDAV.AudioPlayer.Options
{
    public class ConnectionSettings : IConnectionSettings
    {
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