using Blazor.WebDAV.AudioPlayer.Components;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.Client;

namespace Blazor.WebDAV.AudioPlayer.Audio
{
    public interface IPlayerFactory
    {
        public IPlayer GetPlayer(IWebDavClient client, IHowl howl);
    }
}