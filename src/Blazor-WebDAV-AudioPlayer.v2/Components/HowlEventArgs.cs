using System;

namespace Blazor.WebDAV.AudioPlayer.Components
{
    public class HowlEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the sound.
        /// </summary>
        public int SoundId { get; set; }
    }
}