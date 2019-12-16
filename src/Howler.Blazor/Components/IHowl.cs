using System;
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

        ValueTask<int> Play(params Uri[] locations);

        ValueTask<int> Play(params string[] sources);

        ValueTask<int> Play(byte[] audio, string mimeType);

        ValueTask<int> Play(HowlOptions options);

        ValueTask Stop();

        ValueTask Pause(int? soundId = null);

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