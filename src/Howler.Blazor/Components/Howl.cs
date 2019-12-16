using System;
using System.Threading.Tasks;
using Howler.Blazor.Validation;
using Microsoft.JSInterop;

namespace Howler.Blazor.Components
{
    public partial class Howl : IHowl
    {
        private readonly IJSRuntime _runtime;
        private readonly DotNetObjectReference<Howl> _dotNetObjectReference;

        public TimeSpan TotalTime { get; private set; } = TimeSpan.Zero;

        public Howl(IJSRuntime runtime)
        {
            Guard.NotNull(runtime, nameof(runtime));

            _runtime = runtime;
            _dotNetObjectReference = DotNetObjectReference.Create(this);
        }

        public ValueTask<int> Play(Uri location)
        {
            Guard.NotNull(location, nameof(location));

            return Play(location.ToString());
        }

        public ValueTask<int> Play(string location)
        {
            Guard.NotNullOrEmpty(location, nameof(location));

            var options = new HowlOptions
            {
                Sources = new[] { location }
            };

            return Play(options);
        }

        public ValueTask<int> Play(byte[] audio, string mimetype)
        {
            Guard.NotNull(audio, nameof(audio));
            Guard.NotNullOrEmpty(mimetype, nameof(mimetype));

            // http://www.iandevlin.com/blog/2012/09/html5/html5-media-and-data-uri/
            var audioAsBase64 = Convert.ToBase64String(audio);
            string html5AudioUrl = $"data:{mimetype};base64,{audioAsBase64}";

            var options = new HowlOptions
            {
                Sources = new[] { html5AudioUrl }
            };

            return _runtime.InvokeAsync<int>("howl.play", _dotNetObjectReference, options);
        }

        public ValueTask<int> Play(HowlOptions options)
        {
            Guard.NotNull(options, nameof(options));

            return _runtime.InvokeAsync<int>("howl.play", _dotNetObjectReference, options);
        }

        public ValueTask Stop()
        {
            return _runtime.InvokeVoidAsync("howl.stop");
        }

        public ValueTask Pause()
        {
            return _runtime.InvokeVoidAsync("howl.pause");
        }

        public ValueTask Seek(TimeSpan position)
        {
            return _runtime.InvokeVoidAsync("howl.seek", position.TotalSeconds);
        }
        public ValueTask<bool> IsPlaying()
        {
            return _runtime.InvokeAsync<bool>("howl.getIsPlaying", _dotNetObjectReference);
        }

        public async ValueTask<TimeSpan> GetCurrentTime()
        {
            int timeInSeconds = await _runtime.InvokeAsync<int>("howl.getCurrentTime");
            return TimeSpan.FromSeconds(timeInSeconds);
        }

        public async ValueTask<TimeSpan> GetTotalTime()
        {
            int timeInSeconds = await _runtime.InvokeAsync<int>("howl.getTotalTime");
            return TimeSpan.FromSeconds(timeInSeconds);
        }

        public ValueTask<bool> IsCodecSupported(string extension)
        {
            return _runtime.InvokeAsync<bool>("howl.isCodecSupported", extension);
        }

        public ValueTask<string[]> GetCodecs()
        {
            return _runtime.InvokeAsync<string[]>("howl.getCodecs");
        }
    }
}