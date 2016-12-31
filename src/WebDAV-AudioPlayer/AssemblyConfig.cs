using System;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.IO;

namespace WebDav.AudioPlayer
{
    public sealed class AssemblyConfig : AssemblyConfigRepository<AssemblyConfig>, IConnectionSettings
    {
        public Uri StorageUri { get; set; }

        public string RootFolder { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public void SetPassword(string newPassword)
        {
            Password = SecurePasswordHelper.Encrypt(newPassword);
        }

        public string GetPassword()
        {
            return SecurePasswordHelper.Decrypt(Password);
        }
    }
}