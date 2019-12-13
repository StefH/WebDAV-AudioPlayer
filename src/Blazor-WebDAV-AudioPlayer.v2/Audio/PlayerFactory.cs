using Blazor.WebDAV.AudioPlayer.Components;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.Client;

namespace Blazor.WebDAV.AudioPlayer.Audio
{
    public class PlayerFactory : IPlayerFactory
    {
        //private readonly IWebDavClient _client;
        //private readonly IHowl _howl;

        //public PlayerFactory(IWebDavClient client, IHowl howl)
        //{
        //    _client = client;
        //    _howl = howl;
        //}

        public IPlayer GetPlayer(IWebDavClient client, IHowl howl)
        {
            return new Player(client, howl);
        }
    }
}