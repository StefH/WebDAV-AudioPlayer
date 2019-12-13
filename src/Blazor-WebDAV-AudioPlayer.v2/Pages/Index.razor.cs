using Blazor.WebDAV.AudioPlayer.Audio;
using Blazor.WebDAV.AudioPlayer.Client;
using Blazor.WebDAV.AudioPlayer.Components;
using Blazor.WebDAV.AudioPlayer.Models;
using Blazor.WebDAV.AudioPlayer.Options;
using Blazor.WebDAV.AudioPlayer.TreeComponent;
using ByteSizeLib;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace Blazor.WebDAV.AudioPlayer.Pages
{
    public class IndexBase : ComponentBase, IDisposable
    {
        private const string TIME_ZERO = "00:00:00";

        [Inject]
        protected IOptions<ConnectionSettings> Options { get; set; }

        [Inject]
        protected IWebDavClientFactory Factory { get; set; }

        [Inject]
        protected IPlayerFactory PlayerFactory { get; set; }

        [Inject]
        protected IHowl Howl { get; set; }

        protected IPlayer Player { get; set; }

        protected TreeNode<ResourceItem> Root { get; private set; }

        protected List<PlayListItem> PlayListItems { get; private set; } = new List<PlayListItem>();

        protected PlayListItem SelectedPlayListItem { get; private set; }

        protected ResourceLoadStatus Status { get; private set; }

        protected string Logging { get; private set; } = string.Empty;

        protected string CurrentTime { get; set; } = TIME_ZERO;

        protected string TotalTime { get; set; } = TIME_ZERO;

        protected bool IsPlaying { get; set; }

        protected int SliderMax { get; set; } = 1;

        protected int SliderValue { get; set; }

        protected bool SliderEnabled { get; set; } = false;

        private IWebDavClient _client;

        private Timer _timer;

        protected override async Task OnInitializedAsync()
        {
            // await base.OnInitializedAsync();

            //_client = Factory.GetClient();
            // Log($"Using : '{_player.SoundOut}-SoundOut'");

            Root = new TreeNode<ResourceItem>
            {
                Item = new ResourceItem
                {
                    DisplayName = Options.Value.RootFolder,
                    FullPath = OnlinePathBuilder.Combine(Options.Value.StorageUri, Options.Value.RootFolder)
                }
            };
        }

        /// <summary>
        /// Unhandled exception. System.InvalidOperationException: JavaScript interop calls cannot be issued at this time. This is because the component is being statically rendererd. When prerendering is enabled, JavaScript interop calls can only be performed during the OnAfterRenderAsync lifecycle method.
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            var codecs = await Howl.GetCodecs();
            Log($"Supported codecs: {string.Join(", ", codecs)}");

            _client = Factory.GetClient(codecs);

            Player = PlayerFactory.GetPlayer(_client, Howl);
            Player.Log = Log;
            Player.PlayStarted = (selectedIndex, resourceItem) =>
            {
                Log($"PlayStarted - {resourceItem.DisplayName}");

                SelectedPlayListItem = PlayListItems[selectedIndex];
                CurrentTime = TIME_ZERO;

                var totalTime = Player.TotalTime;
                TotalTime = $@"{totalTime:hh\:mm\:ss}";

                SliderMax = (int)totalTime.TotalSeconds;
                SliderEnabled = true;

                PlayListItems[selectedIndex].Bitrate = $"{resourceItem.MediaDetails.BitrateKbps}";
                PlayListItems[selectedIndex].Length = TotalTime;
            };
            Player.PlayContinue = resourceItem =>
            {
                Log($"PlayContinue - {resourceItem.DisplayName}");
            };
            Player.PlayPaused = resourceItem =>
            {
                Log($"PlayPaused - {resourceItem.DisplayName}");
            };
            Player.PlayStopped = () =>
            {
                // Log($"PlayStopped");
                SliderMax = 1;
                SliderValue = 0;
                SliderEnabled = false;
                CurrentTime = TIME_ZERO;
            };

            //_player = new Player(_client, Howl)
            //{
            //    Log = Log,

            //    PlayStarted = (selectedIndex, resourceItem) =>
            //    {
            //        Log($"PlayStarted - {resourceItem.DisplayName}");

            //        SelectedPlayListItem = PlayListItems[selectedIndex];
            //        CurrentTime = TIME_ZERO;

            //        var totalTime = _player.TotalTime;
            //        TotalTime = $@"{totalTime:hh\:mm\:ss}";

            //        SliderMax = (int)totalTime.TotalSeconds;
            //        SliderEnabled = _player.CanSeek;

            //        PlayListItems[selectedIndex].Bitrate = $"{resourceItem.MediaDetails.BitrateKbps}";
            //        PlayListItems[selectedIndex].Length = TotalTime;
            //    },
            //    PlayContinue = resourceItem =>
            //    {
            //        Log($"PlayContinue - {resourceItem.DisplayName}");
            //    },
            //    PlayPaused = resourceItem =>
            //    {
            //        Log($"PlayPaused - {resourceItem.DisplayName}");
            //    },
            //    PlayStopped = () =>
            //    {
            //        // Log($"PlayStopped");
            //        SliderMax = 1;
            //        SliderValue = 0;
            //        SliderEnabled = false;
            //        CurrentTime = TIME_ZERO;
            //    },
            //    //DoubleClickFolderAndPlayFirstSong = async resourceItem =>
            //    //{
            //    //    Log($"DoubleClickFolderAndPlayFirstSong - {resourceItem.DisplayName}");
            //    //    //var nodeToDoubleClick = treeView.Nodes.Find(resourceItem.DisplayName, true).FirstOrDefault();
            //    //    //if (nodeToDoubleClick != null)
            //    //    //{
            //    //    //    await FetchChildResourcesAsync(nodeToDoubleClick, resourceItem);
            //    //    //}
            //    //}
            //};

            

            

            _timer = new Timer(async state => { await InvokeAsync(OnTimerCallback); }, null, 0, 249);

            await RefreshTreeAsync();
        }

        protected async Task Refresh()
        {
            await RefreshTreeAsync();
        }

        protected async Task LazyLoad(TreeNode<ResourceItem> treeNode)
        {
            if (treeNode.ChildNodes != null)
            {
                return;
            }

            Status = ResourceLoadStatus.Unknown;
            Status = await _client.FetchChildResourcesAsync(treeNode.Item, CancellationToken.None, treeNode.Item.Level, treeNode.Item.Level);
            if (Status == ResourceLoadStatus.Ok)
            {
                treeNode.ChildNodes = treeNode.Item.Items
                    .Where(resourceItem => resourceItem.IsCollection)
                    .Select(resourceItem => new TreeNode<ResourceItem>
                    {
                        Item = resourceItem
                    }).ToList();
            }
        }

        protected async Task SelectedResourceItemChanged(TreeNode<ResourceItem> treeNode)
        {
            PlayListItems.Clear();
            SelectedPlayListItem = null;

            if (treeNode.Item.Items == null)
            {
                Status = await _client.FetchChildResourcesAsync(treeNode.Item, CancellationToken.None, treeNode.Item.Level, treeNode.Item.Level);
            }

            if (Status == ResourceLoadStatus.Ok)
            {
                Player.Items = treeNode.Item.Items;
                PlayListItems = treeNode.Item.Items
                    .Where(resourceItem => !resourceItem.IsCollection)
                    .Select((resourceItem, index) => new PlayListItem
                    {
                        Index = index,
                        Item = resourceItem,
                        Title = resourceItem.DisplayName,
                        Bitrate = null,
                        Size = resourceItem.ContentLength != null ? ByteSize.FromBytes(resourceItem.ContentLength.Value).ToString("0.00 MB") : string.Empty,
                        Length = null
                    }).ToList();
            }
        }

        protected async Task ClickSong(PlayListItem item)
        {
            if (item.Index != SelectedPlayListItem?.Index)
            {
                SelectedPlayListItem = item;

                Log($"ClickSong - {item.Title}");

                await Player.PlayAsync(item.Index, CancellationToken.None);
            }
        }

        protected async Task SliderValueChanged(int value)
        {
            var time = TimeSpan.FromSeconds(value);
            Log($@"Jump to '{time:hh\:mm\:ss}'");

            await Player.Seek(time);
        }

        protected async Task Play()
        {
            if (SelectedPlayListItem == null)
            {
                SelectedPlayListItem = PlayListItems.FirstOrDefault();
            }

            if (SelectedPlayListItem != null)
            {
                await Player.PlayAsync(SelectedPlayListItem.Index, CancellationToken.None);
            }
        }

        protected async Task Stop()
        {
            await Player.Stop(false);
        }

        protected async Task Pause()
        {
            await Player.Pause();
        }

        protected async Task Previous()
        {
            await Player.PlayPreviousAsync(CancellationToken.None);
        }

        protected async Task Next()
        {
            await Player.PlayNextAsync(CancellationToken.None);
        }

        protected void ClearLogging()
        {
            Logging = string.Empty;
        }

        private void Log(string text)
        {
            Logging = $"{DateTime.Now} - {text}\r\n" + Logging;
        }

        private async Task OnTimerCallback()
        {
            if (Player == null)
            {
                return;
            }

            var currentTime = await Player.GetCurrentTime();
            CurrentTime = $@"{currentTime:hh\:mm\:ss}";

            if (await Player.GetIsPlaying())
            {
                SliderValue = (int)currentTime.TotalSeconds;

                if (currentTime.Add(TimeSpan.FromMilliseconds(500)) > Player.TotalTime)
                {
                    await Player.PlayNextAsync(CancellationToken.None);
                }
            }

            StateHasChanged();
        }

        private async Task RefreshTreeAsync()
        {
            // await _player?.Stop(true);

            Status = ResourceLoadStatus.Unknown;
            Status = await _client.FetchChildResourcesAsync(Root.Item, CancellationToken.None, 0);
            if (Status == ResourceLoadStatus.Ok)
            {
                Root.ChildNodes = Root.Item.Items.Select(resourceItem => new TreeNode<ResourceItem>
                {
                    Item = resourceItem
                }).ToList();
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}