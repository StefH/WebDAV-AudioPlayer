using System;
using System.Collections.Generic;
using System.Threading;
using NAudio.Wave;
using WebDav.MP3Player.Client;
using WebDav.MP3Player.Models;
using WebDav.MP3Player.Util;

namespace WebDav.MP3Player.Audio
{
    internal class Player
    {
        private readonly WebDAVClient _client;
        private readonly IWavePlayer _waveOutDevice;
        private readonly FixedSizedQueue<ResourceItem> _resourceItemQueue;

        private List<ResourceItem> _items;
        private WaveStream _waveStream;

        public Action<string> Log;
        public Action<string> UpdatePlaying;
        public Action<string> UpdateCurrentTime;
        public Action<string> UpdateTotalTime;

        public List<ResourceItem> Items
        {
            get
            {
                return _items;
            }

            set
            {
                Stop(true);
                _items = value;
            }
        }

        public int SelectedIndex { get; private set; }

        public PlaybackState PlaybackState
        {
            get
            {
                return _waveOutDevice.PlaybackState;
            }
        }

        public TimeSpan CurrentTime
        {
            get
            {
                return _waveOutDevice.PlaybackState == PlaybackState.Stopped || _waveStream == null ? TimeSpan.Zero : _waveStream.CurrentTime;
            }
        }

        public TimeSpan TotalTime
        {
            get
            {
                return _waveStream != null ? _waveStream.TotalTime : TimeSpan.Zero;
            }
        }

        public Player(WebDAVClient client)
        {
            _client = client;

            _resourceItemQueue = new FixedSizedQueue<ResourceItem>(3, (resourceItem, size) =>
            {
                Log(string.Format("Disposing : '{0}'", resourceItem.DisplayName));
                if (resourceItem.Stream != null)
                {
                    resourceItem.Stream.Dispose();
                    resourceItem.Stream = null;
                }
            });

            _waveOutDevice = new WaveOut();
        }

        public async void Play(int index, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            SelectedIndex = index;
            Stop(false);

            var resourceItem = Items[index];

            Log(string.Format(@"Reading : '{0}'", resourceItem.DisplayName));
            var status = await _client.GetStreamAsync(resourceItem, cancellationToken);
            if (status == WebDAVClient.ResourceLoadStatus.NoFile || status == WebDAVClient.ResourceLoadStatus.OperationCanceled)
            {
                Log(string.Format(@"Reading error : {0}", status));
                return;
            }

            Log(string.Format(@"Reading done : {0}", status));

            _resourceItemQueue.Enqueue(resourceItem);

            UpdatePlaying(string.Format(@"Playing : '{0}'", resourceItem.DisplayName));

            //_waveStream = new Mp3FileReader(resourceItem.Stream, wave => new DmoMp3FrameDecompressor(wave));

            if (resourceItem.DisplayName.EndsWith(".mp3"))
                _waveStream = new Mp3FileReader(resourceItem.Stream);
            else
                _waveStream = new StreamMediaFoundationReader(resourceItem.Stream);

            _waveOutDevice.Init(_waveStream);
            _waveOutDevice.Play();

            UpdateTotalTime(string.Format(@"{0:hh\:mm\:ss}", _waveStream.TotalTime));

            // Preload Next
            PreloadNext(cancellationToken);
        }

        private async void PreloadNext(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            int nextIndex = SelectedIndex + 1;
            if (nextIndex < Items.Count)
            {
                var resourceItem = Items[nextIndex];
                Log(string.Format("Preloading : '{0}'", resourceItem.DisplayName));
                var status = await _client.GetStreamAsync(resourceItem, cancellationToken);
                if (status == WebDAVClient.ResourceLoadStatus.NoFile || status == WebDAVClient.ResourceLoadStatus.OperationCanceled)
                {
                    Log(string.Format(@"Preloading error : {0}", status));
                    return;
                }

                Log(string.Format(@"Preloading done : {0}", status));
                _resourceItemQueue.Enqueue(resourceItem);
            }
        }

        public void Next(CancellationToken cancelAction)
        {
            int nextIndex = SelectedIndex + 1;
            if (nextIndex < Items.Count)
                Play(nextIndex, cancelAction);
            else
                Stop(true);
        }

        public void Previous(CancellationToken cancelAction)
        {
            int previousIndex = SelectedIndex - 1;
            if (previousIndex >= 0)
                Play(previousIndex, cancelAction);
            else
                Stop(true);
        }

        public void Stop(bool force)
        {
            if (_waveOutDevice != null)
            {
                UpdateTotalTime(@"00:00:00");
                UpdateCurrentTime(@"00:00:00");
                UpdatePlaying(@"Stopped");

                _waveOutDevice.Stop();

                if (force)
                {
                    _resourceItemQueue.Clear();
                }
            }

            if (_waveStream != null)
            {
                _waveStream.Dispose();
                _waveStream = null;
            }
        }

        public void Pause()
        {
            var resourceItem = Items[SelectedIndex];

            if (_waveOutDevice.PlaybackState == PlaybackState.Playing)
            {
                UpdatePlaying(string.Format(@"Pausing : '{0}'", resourceItem.DisplayName));
                _waveOutDevice.Pause();
            }
            else if (_waveOutDevice.PlaybackState == PlaybackState.Paused)
            {
                UpdatePlaying(string.Format(@"Playing : '{0}'", resourceItem.DisplayName));
                _waveOutDevice.Play();
            }
        }
    }
}