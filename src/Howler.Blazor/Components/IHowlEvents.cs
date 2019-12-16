using Howler.Blazor.Components.Events;
using System;

namespace Howler.Blazor.Components
{
    public interface IHowlEvents
    {
        /// <summary>
        /// Fires when the sound begins playing.
        /// </summary>
        event Action<HowlPlayEventArgs> OnPlay;

        /// <summary>
        /// Fires when the sound has been stopped.
        /// </summary>
        event Action<HowlEventArgs> OnStop;

        /// <summary>
        /// Fires when the sound finishes playing (if it is looping, it'll fire at the end of each loop).
        /// </summary>
        event Action<HowlEventArgs> OnEnd;

        /// <summary>
        /// Fires when the sound is unable to load.
        /// </summary>
        event Action<HowlErrorEventArgs> OnLoadError;

        /// <summary>
        /// Fires when the sound is unable to play.
        /// </summary>
        event Action<HowlErrorEventArgs> OnPlayError;

        /*
         * onload Function
Fires when the sound is loaded.

onpause Function
Fires when the sound has been paused. The first parameter is the ID of the sound.

onmute Function
Fires when the sound has been muted/unmuted. The first parameter is the ID of the sound.

onvolume Function
Fires when the sound's volume has changed. The first parameter is the ID of the sound.

onrate Function
Fires when the sound's playback rate has changed. The first parameter is the ID of the sound.

onseek Function
Fires when the sound has been seeked. The first parameter is the ID of the sound.

onfade Function
Fires when the current sound finishes fading in/out. The first parameter is the ID of the sound.

onunlock Function
Fires when audio has been automatically unlocked through a touch/click event.*/
    }
}