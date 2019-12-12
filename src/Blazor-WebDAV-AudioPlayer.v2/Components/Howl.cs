using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazor.WebDAV.AudioPlayer.Components
{
    public class Howl : IHowl
    {
        private readonly DotNetObjectReference<Howl> dotNetObjectReference;

        private IJSRuntime _runtime { get; }

        public TimeSpan TotalTime { get; private set; } = TimeSpan.Zero;

        public Action<TimeSpan> OnPlay { get; set; }

        public Howl(IJSRuntime runtime)
        {
            _runtime = runtime;

            dotNetObjectReference = DotNetObjectReference.Create(this);
        }

        [JSInvokable]
        public void OnPlayCallback(int durationInSeconds)
        {
            TotalTime = TimeSpan.FromSeconds(durationInSeconds);

            OnPlay?.Invoke(TotalTime);
        }

        [JSInvokable]
        public void OnStopCallback()
        {
        }

        public async Task<bool> IsPlaying()
        {
            return await _runtime.InvokeAsync<bool>("howl.getIsPlaying", dotNetObjectReference);
        }

        public async Task<int> Play(byte[] audio, string extension)
        {
            var audioAsBase64 = Convert.ToBase64String(audio);

            return await _runtime.InvokeAsync<int>("howl.play", dotNetObjectReference, audioAsBase64, extension);
        }

        public async Task<int> Play(string location)
        {
            return await _runtime.InvokeAsync<int>("howl.play", dotNetObjectReference, location);
        }

        public async Task Stop()
        {
            await _runtime.InvokeAsync<dynamic>("howl.stop");
        }

        public async Task Pause()
        {
            await _runtime.InvokeAsync<dynamic>("howl.pause");
        }

        public async Task Seek(TimeSpan position)
        {
            await _runtime.InvokeAsync<dynamic>("howl.seek", position.TotalSeconds);
        }

        public async Task<TimeSpan> GetCurrentTime()
        {
            int timeInSeconds = await _runtime.InvokeAsync<int>("howl.getCurrentTime");
            return TimeSpan.FromSeconds(timeInSeconds);
        }

        public async Task<TimeSpan> GetTotalTime()
        {
            int timeInSeconds = await _runtime.InvokeAsync<int>("howl.getTotalTime");
            return TimeSpan.FromSeconds(timeInSeconds);
        }
    }
}