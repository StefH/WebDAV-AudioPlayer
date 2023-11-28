using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebDav.AudioPlayer.Models;

namespace WebDav.AudioPlayer.Audio
{
    public interface IPlayer : IDisposable
    {
        Func<ResourceItem, Task> DoubleClickFolderAndPlayFirstSong { get; set; }
        List<ResourceItem> Items { get; set; }
        Action<string> Log { get; set; }
        Action<ResourceItem> PlayContinue { get; set; }
        Action<ResourceItem> PlayPause { get; set; }
        Action<int, ResourceItem> PlayStart { get; set; }
        Action<int, ResourceItem> PlayEnd { get; set; }
        Action PlayStop { get; set; }
        int SelectedIndex { get; }
        ResourceItem SelectedResourceItem { get; set; }
        TimeSpan TotalTime { get; }

        Task<string[]> GetCodecs();
        Task<TimeSpan> GetCurrentTime();
        Task<bool> GetIsPlaying();
        Task Pause();
        Task PlayAsync(int index, CancellationToken cancellationToken);
        Task PlayNextAsync(CancellationToken cancelAction);
        Task PlayPreviousAsync(CancellationToken cancelAction);
        Task Seek(TimeSpan position);
        Task StopAsync(bool force);
    }
}