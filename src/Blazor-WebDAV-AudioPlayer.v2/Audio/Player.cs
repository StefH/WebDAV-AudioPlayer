using Howler.Blazor.Components;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace WebDav.AudioPlayer.Audio
{
    internal class Player : IPlayer
    {
        private readonly IConnectionSettings _settings;
        private readonly IMemoryCache _cache;
        private readonly IWebDavClient _client;
        private readonly IHowl _howl;

        // private readonly FixedSizedQueue<ResourceItem> _resourceItemQueue;

        private List<ResourceItem> _items;
        private string[] _codecs;

        public ResourceItem SelectedResourceItem { get; set; }

        public Action<string> Log { get; set; }
        public Action<int, ResourceItem> PlayStarted { get; set; }
        public Action<ResourceItem> PlayPaused { get; set; }
        public Action<ResourceItem> PlayContinue { get; set; }
        public Func<ResourceItem, Task> DoubleClickFolderAndPlayFirstSong { get; set; }
        public Action PlayStopped { get; set; }

        public List<ResourceItem> Items
        {
            get => _items;

            set
            {
                Stop(true);
                _items = value;
            }
        }

        public int SelectedIndex { get; private set; } = -1;

        private TimeSpan _totalTime = TimeSpan.Zero;
        public TimeSpan TotalTime
        {
            get
            {
                return _totalTime;
            }
        }

        public Player(IConnectionSettings settings, IMemoryCache cache, IWebDavClient client, IHowl howl)
        {
            _settings = settings;
            _cache = cache;
            _client = client;
            _howl = howl;

            //_resourceItemQueue = new FixedSizedQueue<ResourceItem>(3, (resourceItem, size) =>
            //{
            //    Log($"Disposing : '{resourceItem.DisplayName}'");
            //    if (resourceItem.Stream != null)
            //    {
            //        resourceItem.Stream.Close();
            //        resourceItem.Stream.Dispose();
            //        resourceItem.Stream = null;
            //    }
            //});

            _howl.OnPlay += (e) =>
            {
                _totalTime = e.TotalTime;

                PlayStarted(SelectedIndex, SelectedResourceItem);
            };
        }

        public async Task<bool> GetIsPlaying()
        {
            return await _howl?.IsPlaying() == true;
        }

        public async Task<TimeSpan> GetCurrentTime()
        {
            return _howl != null ? await _howl?.GetCurrentTime() : TimeSpan.Zero;
        }

        public async Task PlayAsync(int index, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            bool sameSong = index == SelectedIndex;
            SelectedIndex = index;
            SelectedResourceItem = Items[index];

            // If same song and stream is loaded, just Seek start and start play.
            if (sameSong && SelectedResourceItem.Stream != null && await GetIsPlaying())
            {
                await Seek(TimeSpan.Zero);
                return;
            }

            await Stop(false);

            Log($@"Reading : '{SelectedResourceItem.DisplayName}'");

            if (!_cache.TryGetValue(SelectedResourceItem.Id, out ResourceItem cachedResourceItem))
            {
                var status = await _client.GetStreamAsync(SelectedResourceItem, cancellationToken);
                if (status != ResourceLoadStatus.Ok && status != ResourceLoadStatus.StreamExisting)
                {
                    Log($@"Reading error : {status}");
                    return;
                }

                Log($@"Reading done : {status}");

                _cache.Set(SelectedResourceItem.Id, SelectedResourceItem, new MemoryCacheEntryOptions { Size = 1 });
            }

            //var status = await _client.GetStreamAsync(SelectedResourceItem, cancellationToken);
            //if (status != ResourceLoadStatus.Ok && status != ResourceLoadStatus.StreamExisting)
            //{
            //    Log($@"Reading error : {status}");
            //    return;
            //}

            //Log($@"Reading done : {status}");

            //_resourceItemQueue.Enqueue(SelectedResourceItem);

            //string path = SelectedResourceItem.FullPath.ToString().Replace(_settings.StorageUri.ToString(), "_sounds_");
            await _howl.Play($"sounds/{SelectedResourceItem.Id}{SelectedResourceItem.Extension}");

            // Preload Next
            await PreloadNextAsync(cancellationToken);
        }

        //private byte[] ReadStreamAsBytes(Stream input)
        //{
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        input.CopyTo(memoryStream);
        //        input.Seek(0, SeekOrigin.Begin);
        //        return memoryStream.ToArray();
        //    }
        //}

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

                if (!_cache.TryGetValue(resourceItem.Id, out ResourceItem cachedResourceItem))
                {
                    var status = await _client.GetStreamAsync(resourceItem, cancellationToken);
                    if (status != ResourceLoadStatus.Ok && status != ResourceLoadStatus.StreamExisting)
                    {
                        Log($@"Preloading error : {status}");
                        return;
                    }

                    Log($@"Preloading done : {status}");

                    _cache.Set(resourceItem.Id, resourceItem, new MemoryCacheEntryOptions { Size = 1 });
                }

                //var status = await _client.GetStreamAsync(resourceItem, cancellationToken);
                //if (status != ResourceLoadStatus.Ok && status != ResourceLoadStatus.StreamExisting)
                //{
                //    Log($@"Preloading error : {status}");
                //    return;
                //}

                //Log($@"Preloading done : {status}");
                //_resourceItemQueue.Enqueue(resourceItem);
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
                await Stop(true);
            }
        }

        public async Task Stop(bool force)
        {
            await _howl.Stop();
            PlayStopped();

            if (force)
            {
                //_resourceItemQueue.Clear();
                //_cache.Remove
            }
        }

        public async Task Pause()
        {
            var resourceItem = Items[SelectedIndex];

            if (await _howl.IsPlaying())
            {
                PlayPaused(resourceItem);
            }
            else
            {
                PlayContinue(resourceItem);
            }

            await _howl.Pause();
        }

        public async Task Seek(TimeSpan position)
        {
            await _howl.Seek(position);
        }

        public async Task<string[]> GetCodecs()
        {
            if (_codecs == null)
            {
                _codecs = await _howl.GetCodecs();
            }

            return _codecs;
        }

        public void Dispose()
        {
            Stop(true);
        }
    }
}