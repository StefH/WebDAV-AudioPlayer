﻿using Howler.Blazor.Components.Events;
using Microsoft.JSInterop;
using System;

namespace Howler.Blazor.Components
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