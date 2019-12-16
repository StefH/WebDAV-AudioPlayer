﻿using System;
using System.Threading.Tasks;

namespace Howler.Blazor.Components
{
    public interface IHowl : IHowlEvents
    {
        #region Properties
        TimeSpan TotalTime { get; }
        #endregion

        #region  Methods
        ValueTask<bool> IsPlaying();

        ValueTask<int> Play(Uri location);

        ValueTask<int> Play(string location);

        ValueTask<int> Play(byte[] audio, string mimeType);

        ValueTask<int> Play(HowlOptions options);

        ValueTask Stop();

        ValueTask Pause();

        ValueTask Seek(TimeSpan position);

        ValueTask<TimeSpan> GetCurrentTime();

        ValueTask<TimeSpan> GetTotalTime();

        ValueTask<bool> IsCodecSupported(string extension);
        #endregion

        #region Global Methods
        ValueTask<string[]> GetCodecs();
        #endregion
    }
}