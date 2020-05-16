using WebDav.AudioPlayer.Models;

namespace Blazor.WebDAV.AudioPlayer.Models
{
    public class PlayListItem
    {
        public int Index { get; set; }

        public string Title { get; set; }

        public string Size { get; set; }

        public string Length { get; set; }

        public string Bitrate { get; set; }

        public ResourceItem Item { get; set; }
    }
}