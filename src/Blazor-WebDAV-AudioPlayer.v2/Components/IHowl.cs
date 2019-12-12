using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Blazor.WebDAV.AudioPlayer.Components
{
    public interface IHowl
    {
        #region Events
        Action<TimeSpan> OnPlay { get; set; }
        #endregion

        #region Properties
        TimeSpan TotalTime { get; }
        #endregion

        #region 
        Task<bool> IsPlaying();

        Task<int> Play(byte[] audio, string mimeType);

        Task<int> Play(string location);

        Task Stop();

        Task Pause();

        Task Seek(TimeSpan position);

        Task<TimeSpan> GetCurrentTime();

        Task<TimeSpan> GetTotalTime();
        #endregion
    }
}