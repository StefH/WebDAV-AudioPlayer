using System;

namespace WebDav.AudioPlayer.Client
{
    public interface IConnectionSettings
    {
        Uri StorageUri { get; }

        string UserName { get; }

        string RootFolder { get; }

        string GetPassword();
    }
}
