using CSCore;
using CSCore.Codecs.AAC;
using CSCore.Codecs.FLAC;
using CSCore.Codecs.MP3;
using CSCore.Codecs.OGG;
using CSCore.Codecs.OPUS;
using CSCore.Codecs.WAV;
using CSCore.Codecs.WMA;
using CSCore.SoundOut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;
using WebDav.AudioPlayer.Util;

namespace WebDav.AudioPlayer.Audio
{
    internal class Player : IDisposable
    {
        private static readonly string[] DefaultAudioExtensions = { "aac", "flac", "m4a", "mp3", "mp4", "ogg", "opus", "wav", "wma" };

        private readonly IWebDavClient _client;

        private readonly FixedSizedQueue<ResourceItem> _resourceItemQueue;

        private readonly ISoundOut _soundOut;
        private IWaveSource _waveSource;
        private List<ResourceItem> _items;

        public bool CanSeek => _waveSource.CanSeek;

        public Action<string> Log;
        public Action<Player, int, ResourceItem> PlayStarted;
        public Action<ResourceItem> PlayPaused;
        public Action<ResourceItem> PlayContinue;
        public Func<ResourceItem, Task> DoubleClickFolderAndPlayFirstSong;
        public Action PlayStopped;

        public List<ResourceItem> Items
        {
            get => _items;

            set
            {
                Stop(true);
                _items = value.Where(r => IsAudioFile(r)).ToList();
            }
        }

        public int SelectedIndex { get; private set; } = -1;

        public PlaybackState PlaybackState => _soundOut?.PlaybackState ?? PlaybackState.Stopped;

        public TimeSpan CurrentTime => _waveSource?.GetPosition() ?? TimeSpan.Zero;

        public TimeSpan TotalTime => _waveSource?.GetLength() ?? TimeSpan.Zero;

        public string SoundOut => _soundOut.GetType().Name;

        public Player(IWebDavClient client)
        {
            _client = client;

            _resourceItemQueue = new FixedSizedQueue<ResourceItem>(3, (resourceItem, size) =>
            {
                Log($"Disposing : '{resourceItem.DisplayName}'");
                if (resourceItem.Stream != null)
                {
                    resourceItem.Stream.Close();
                    resourceItem.Stream.Dispose();
                    resourceItem.Stream = null;
                }
            });

            if (WasapiOut.IsSupportedOnCurrentPlatform)
            {
                _soundOut = new WasapiOut();
            }
            else
            {
                _soundOut = new DirectSoundOut();
            }
        }

        public async Task PlayAsync(int index, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            bool sameSong = index == SelectedIndex;
            SelectedIndex = index;
            var resourceItem = Items[index];

            // If paused, just unpause and play
            if (PlaybackState == PlaybackState.Paused)
            {
                Pause();
                return;
            }

            // If same song and stream is loaded, just JumpTo start and start play.
            if (sameSong && resourceItem.Stream != null && PlaybackState == PlaybackState.Playing)
            {
                JumpTo(TimeSpan.Zero);
                return;
            }

            Stop(false);

            Log($@"Reading : '{resourceItem.DisplayName}'");
            var status = await _client.GetStreamAsync(resourceItem, cancellationToken);
            if (status != ResourceLoadStatus.Ok && status != ResourceLoadStatus.StreamExisting)
            {
                Log($@"Reading error : {status}");
                return;
            }

            Log($@"Reading done : {status}");

            _resourceItemQueue.Enqueue(resourceItem);

            string extension = new FileInfo(resourceItem.DisplayName).Extension.ToLowerInvariant();
            switch (extension)
            {
                case ".wav":
                    _waveSource = new WaveFileReader(resourceItem.Stream);
                    break;

                case ".mp3":
                    _waveSource = new DmoMp3Decoder(resourceItem.Stream);
                    break;

                case ".ogg":
                    _waveSource = new OggSource(resourceItem.Stream).ToWaveSource();
                    break;

                case ".flac":
                    _waveSource = new FlacFile(resourceItem.Stream);
                    break;

                case ".wma":
                    _waveSource = new WmaDecoder(resourceItem.Stream);
                    break;

                case ".aac":
                case ".m4a":
                case ".mp4":
                    _waveSource = new AacDecoder(resourceItem.Stream);
                    break;

                case ".opus":
                    _waveSource = new OpusSource(resourceItem.Stream, resourceItem.MediaDetails.SampleRate, resourceItem.MediaDetails.Channels);
                    break;

                default:
                    throw new NotSupportedException($"Extension '{extension}' is not supported");
            }

            _soundOut.Initialize(_waveSource);
            _soundOut.Play();
            PlayStarted(this, SelectedIndex, resourceItem);

            // Preload Next
            await PreloadNextAsync(cancellationToken);
        }

        private async Task PreloadNextAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            int nextIndex = SelectedIndex + 1;
            if (nextIndex < Items.Count)
            {
                // Loading next resourceItem from this folder
                var resourceItem = Items[nextIndex];
                Log($"Preloading : '{resourceItem.DisplayName}'");
                var status = await _client.GetStreamAsync(resourceItem, cancellationToken);
                if (status != ResourceLoadStatus.Ok && status != ResourceLoadStatus.StreamExisting)
                {
                    Log($@"Preloading error : {status}");
                    return;
                }

                Log($@"Preloading done : {status}");
                _resourceItemQueue.Enqueue(resourceItem);
            }
        }

        private ResourceItem GetNextFolderFromParent(ResourceItem lastResourceItemFromFolder)
        {
            var parent = lastResourceItemFromFolder?.Parent?.Parent;
            if (parent != null)
            {
                var folderFromLastResourceItem = lastResourceItemFromFolder.Parent;
                int indexFromCurrentPlayingFolder = parent.Items.IndexOf(folderFromLastResourceItem);
                int indexFromNextFolder = indexFromCurrentPlayingFolder + 1;
                if (indexFromNextFolder < parent.Items.Count)
                {
                    return parent.Items.ElementAt(indexFromNextFolder);
                }
            }

            return null;
        }

        public async Task PlayNextAsync(CancellationToken cancelAction)
        {
            int nextIndex = SelectedIndex + 1;

            if (nextIndex < Items.Count)
            {
                await PlayAsync(nextIndex, cancelAction);
            }
            else
            {
                var currentResourceItem = Items[SelectedIndex];
                var nextFolderToPlay = GetNextFolderFromParent(currentResourceItem);
                if (nextFolderToPlay != null && DoubleClickFolderAndPlayFirstSong != null)
                {
                    await DoubleClickFolderAndPlayFirstSong(nextFolderToPlay);
                    await PlayAsync(0, cancelAction);
                }
                else
                {
                    Stop(true);
                }
            }
        }

        public async Task PlayPreviousAsync(CancellationToken cancelAction)
        {
            int previousIndex = SelectedIndex - 1;

            if (previousIndex >= 0)
            {
                await PlayAsync(previousIndex, cancelAction);
            }
            else
            {
                Stop(true);
            }
        }

        public void Stop(bool force)
        {
            if (_soundOut != null)
            {
                _soundOut.Stop();
                PlayStopped();

                if (force)
                {
                    _resourceItemQueue.Clear();
                }
            }

            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
        }

        public void Pause()
        {
            var resourceItem = Items[SelectedIndex];

            if (PlaybackState == PlaybackState.Playing)
            {
                _soundOut.Pause();
                PlayPaused(resourceItem);
            }
            else if (PlaybackState == PlaybackState.Paused)
            {
                _soundOut.Play();
                PlayContinue(resourceItem);
            }
        }

        public void JumpTo(TimeSpan position)
        {
            if (PlaybackState == PlaybackState.Playing)
            {
                _soundOut.Stop();
                _waveSource.SetPosition(position);
                _soundOut.Play();
            }
        }

        public void SetVolume(float volume)
        {
            if (PlaybackState == PlaybackState.Playing)
            {
                _soundOut.Volume = volume;
            }
        }

        public void Dispose()
        {
            Stop(true);
            _soundOut.Dispose();
        }

        private static bool IsAudioFile(ResourceItem r)
        {
            return DefaultAudioExtensions.Any(e => r.FullPath.ToString().ToLowerInvariant().EndsWith($".{e}"));
        }
    }
}