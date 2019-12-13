using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazor.Howler.Components
{
    public partial class Howl
    {
        [JSInvokable]
        public void OnPlayCallback(int soundId, int durationInSeconds)
        {
            TotalTime = TimeSpan.FromSeconds(durationInSeconds);

            OnPlay?.Invoke(new HowlPlayEventArgs { SoundId = soundId, TotalTime = TotalTime });
        }

        [JSInvokable]
        public void OnStopCallback(int soundId)
        {
            TotalTime = TimeSpan.Zero;

            OnStop?.Invoke(new HowlEventArgs { SoundId = soundId });
        }
    }
}