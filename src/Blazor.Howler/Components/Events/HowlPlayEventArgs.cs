using System;

namespace Blazor.WebDAV.AudioPlayer.Components
{
    public class HowlPlayEventArgs : HowlEventArgs
    {
        public TimeSpan TotalTime { get; set; }
    }
}