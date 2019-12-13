using Blazor.WebDAV.AudioPlayer.Components;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.Client;

namespace Blazor.WebDAV.AudioPlayer.Audio
{
    public class PlayerFactory : IPlayerFactory
    {
        public IPlayer GetPlayer(IWebDavClient client, IHowl howl)
        {
            return new Player(client, howl);
        }
    }
}