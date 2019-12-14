using Howler.Blazor.Components.Events;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Howler.Blazor.Components
{
    public partial class Howl : IHowl
    {
        private readonly IJSRuntime _runtime;
        private readonly DotNetObjectReference<Howl> dotNetObjectReference;

        public TimeSpan TotalTime { get; private set; } = TimeSpan.Zero;

        public event Action<HowlPlayEventArgs> OnPlay;

        public event Action<HowlEventArgs> OnStop;

        public Howl(IJSRuntime runtime)
        {
            _runtime = runtime;

            dotNetObjectReference = DotNetObjectReference.Create(this);
        }

        public async Task<bool> IsPlaying()
        {
            return await _runtime.InvokeAsync<bool>("howl.getIsPlaying", dotNetObjectReference);
        }

        public async Task<int> Play(byte[] audio, string mimetype)
        {
            // http://www.iandevlin.com/blog/2012/09/html5/html5-media-and-data-uri/
            var audioAsBase64 = Convert.ToBase64String(audio);
            string html5AudioUrl = $"data:{mimetype};base64,{audioAsBase64}";

            return await _runtime.InvokeAsync<int>("howl.play", dotNetObjectReference, html5AudioUrl);
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

        public async Task<bool> IsCodecSupported(string extension)
        {
            return await _runtime.InvokeAsync<bool>("howl.isCodecSupported", extension);
        }

        public async Task<string[]> GetCodecs()
        {
            return await _runtime.InvokeAsync<string[]>("howl.getCodecs");
        }
    }
}