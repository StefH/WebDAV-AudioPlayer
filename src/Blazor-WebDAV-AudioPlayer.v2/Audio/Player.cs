﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazor.WebDAV.AudioPlayer.Constants;
using Howler.Blazor.Components;
using Microsoft.Extensions.Caching.Memory;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace Blazor.WebDAV.AudioPlayer.Audio;

internal class Player : IPlayer
{
    private readonly IMemoryCache _cache;
    private readonly IWebDavClient _client;
    private readonly IHowl _howl;
    private readonly IHowlGlobal _howlGlobal;

    private List<ResourceItem> _items;
    private string[] _codecs;
    private int _soundId;

    public ResourceItem SelectedResourceItem { get; set; }

    public Action<string> Log { get; set; }
    public Action<int, ResourceItem> PlayStart { get; set; }
    public Action<int, ResourceItem> PlayEnd { get; set; }
    public Action<ResourceItem> PlayPause { get; set; }
    public Action<ResourceItem> PlayContinue { get; set; }
    public Func<ResourceItem, Task> DoubleClickFolderAndPlayFirstSong { get; set; }
    public Action PlayStop { get; set; }

    public List<ResourceItem> Items
    {
        get => _items;

        set
        {
            StopAsync(true);

            _items = value;
        }
    }

    public int SelectedIndex { get; private set; } = -1;

    public TimeSpan TotalTime { get; private set; } = TimeSpan.Zero;

    public Player(IMemoryCache cache, IWebDavClient client, IHowl howl, IHowlGlobal howlGlobal)
    {
        _cache = cache;
        _client = client;
        _howl = howl;
        _howlGlobal = howlGlobal;

        _howl.OnPlay += e =>
        {
            TotalTime = e.TotalTime;

            PlayStart(SelectedIndex, SelectedResourceItem);
        };

        _howl.OnEnd += e =>
        {
            PlayEnd(SelectedIndex, SelectedResourceItem);
        };
    }

    public async Task<bool> GetIsPlaying()
    {
        return await _howl.IsPlaying(_soundId);
    }

    public async Task<TimeSpan> GetCurrentTime()
    {
        return await _howl.GetCurrentTime(_soundId);
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

        await StopAsync(false);

        Log($@"Loading : '{SelectedResourceItem.DisplayName}'");

        if (!_cache.TryGetValue(SelectedResourceItem.Id, out ResourceItem cachedResourceItem))
        {
            var status = await _client.GetStreamAsync(SelectedResourceItem, cancellationToken);
            if (status != ResourceLoadStatus.Ok && status != ResourceLoadStatus.StreamExisting)
            {
                Log($@"Loading error : {status}");
                return;
            }

            Log($@"Loading : '{SelectedResourceItem.DisplayName}' is done");

            if (status == ResourceLoadStatus.Ok)
            {
                _cache.Set(SelectedResourceItem.Id, SelectedResourceItem, new MemoryCacheEntryOptions { Size = 1 });
            }
        }

        _soundId = await _howl.Play($"{AudioPlayerConstants.SoundPrefix}{SelectedResourceItem.Id}{SelectedResourceItem.Extension}");

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

            if (!_cache.TryGetValue(resourceItem.Id, out ResourceItem cachedResourceItem))
            {
                var status = await _client.GetStreamAsync(resourceItem, cancellationToken);
                if (status != ResourceLoadStatus.Ok && status != ResourceLoadStatus.StreamExisting)
                {
                    Log($"Preloading error : {status}");
                    return;
                }

                Log($"Preloading : '{resourceItem.DisplayName}' is done");

                if (status == ResourceLoadStatus.Ok)
                {
                    _cache.Set(resourceItem.Id, resourceItem, new MemoryCacheEntryOptions { Size = 1 });
                }
            }
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
                StopAsync(true);
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
            await StopAsync(true);
        }
    }

    public async Task StopAsync(bool force)
    {
        await _howl.Stop(_soundId);
        PlayStop();

        if (force)
        {
            //_resourceItemQueue.Clear();
            //_cache.Remove
        }
    }

    public async Task Pause()
    {
        var resourceItem = Items[SelectedIndex];

        if (await _howl.IsPlaying(_soundId))
        {
            PlayPause(resourceItem);
        }
        else
        {
            PlayContinue(resourceItem);
        }

        await _howl.Pause(_soundId);
    }

    public async Task Seek(TimeSpan position)
    {
        await _howl.Seek(_soundId, position);
    }

    public async Task<string[]> GetCodecs()
    {
        return _codecs ??= await _howlGlobal.GetCodecs();
    }

    public void Dispose()
    {
        StopAsync(true);
    }
}