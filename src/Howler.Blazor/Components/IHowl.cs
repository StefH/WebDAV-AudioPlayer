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
        Task<bool> IsPlaying();

        Task<int> Play(Uri location);

        Task<int> Play(string location);

        Task<int> Play(byte[] audio, string mimeType);

        Task<int> Play(HowlOptions options);

        Task Stop();

        Task Pause();

        Task Seek(TimeSpan position);

        Task<TimeSpan> GetCurrentTime();

        Task<TimeSpan> GetTotalTime();
        #endregion

        #region Global Methods
        Task<string[]> GetCodecs();
        #endregion
    }
}