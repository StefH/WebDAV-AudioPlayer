using Blazor.WebDAV.AudioPlayer.Client;
using Blazor.WebDAV.AudioPlayer.Models;
using Blazor.WebDAV.AudioPlayer.Options;
using Blazor.WebDAV.AudioPlayer.TreeComponent;
using ByteSizeLib;
using CSCore.SoundOut;
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
    public class IndexBase : ComponentBase
    {
        private const string TIME_ZERO = "00:00:00";

        [Inject]
        protected IOptions<ConnectionSettings> Options { get; set; }

        [Inject]
        protected IWebDavClientFactory Factory { get; set; }

        protected TreeNode<ResourceItem> Root { get; private set; }

        protected List<PlayListItem> PlayListItems { get; private set; } = new List<PlayListItem>();

        protected ResourceLoadStatus Status { get; private set; }

        protected string Logging { get; private set; } = string.Empty;

        private IWebDavClient _client;

        private Player _player;

        private Timer _timer;

        public string CurrentTime { get; set; } = TIME_ZERO;

        public string TotalTime { get; set; } = TIME_ZERO;

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();

            _client = Factory.GetClient();

            _player = new Player(_client)
            {
                Log = Log,

                PlayStarted = (selectedIndex, resourceItem) =>
                {
                    Log($"PlayStarted - {resourceItem.DisplayName}");
                    //string bitrate = $"{resourceItem.MediaDetails.BitrateKbps}";
                    //string text = updateTitle(resourceItem, "Playing");
                    //textBoxSong.Text = text;

                    CurrentTime = TIME_ZERO;
                    TotalTime = $@"{_player.TotalTime:hh\:mm\:ss}";

                    //trackBarSong.Maximum = (int)_player.TotalTime.TotalSeconds;
                    //trackBarSong.Enabled = _player.CanSeek;

                    //listView.SetCells(selectedIndex, $@"{_player.TotalTime:h\:mm\:ss}", bitrate);
                    //listView.SetSelectedIndex(selectedIndex);

                    PlayListItems[selectedIndex].Bitrate = $"{resourceItem.MediaDetails.BitrateKbps}";
                    PlayListItems[selectedIndex].Length = $@"{_player.TotalTime:hh\:mm\:ss}";
                },
                PlayContinue = resourceItem =>
                {
                    Log($"PlayContinue - {resourceItem.DisplayName}");
                    //string text = updateTitle(resourceItem, "Playing");
                    //textBoxSong.Text = text;
                },
                PlayPaused = resourceItem =>
                {
                    Log($"PlayPaused - {resourceItem.DisplayName}");
                    //string text = updateTitle(resourceItem, "Pausing");
                    //textBoxSong.Text = text;
                },
                PlayStopped = () =>
                {
                    // Log($"PlayStopped");
                    //trackBarSong.Value = 0;
                    //trackBarSong.Maximum = 1;
                    CurrentTime = TIME_ZERO;
                    //Text = @"WebDAV-AudioPlayer";
                },
                //DoubleClickFolderAndPlayFirstSong = async resourceItem =>
                //{
                //    Log($"DoubleClickFolderAndPlayFirstSong - {resourceItem.DisplayName}");
                //    //var nodeToDoubleClick = treeView.Nodes.Find(resourceItem.DisplayName, true).FirstOrDefault();
                //    //if (nodeToDoubleClick != null)
                //    //{
                //    //    await FetchChildResourcesAsync(nodeToDoubleClick, resourceItem);
                //    //}
                //}
            };

            Log(_player.SoundOut);

            _timer = new Timer(async state => { await InvokeAsync(UpdateCounter); }, null, 0, 249);

            Root = new TreeNode<ResourceItem>
            {
                Item = new ResourceItem
                {
                    DisplayName = Options.Value.RootFolder,
                    FullPath = OnlinePathBuilder.Combine(Options.Value.StorageUri, Options.Value.RootFolder)
                }
            };

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

            if (treeNode.Item.Items == null)
            {
                Status = await _client.FetchChildResourcesAsync(treeNode.Item, CancellationToken.None, treeNode.Item.Level, treeNode.Item.Level);
            }

            if (Status == ResourceLoadStatus.Ok)
            {
                _player.Items = treeNode.Item.Items;
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

        public async Task ClickSong(PlayListItem item)
        {
            Log($"ClickSong - {item.Title}");
            // _player.Stop(true);

            await _player.PlayAsync(item.Index, CancellationToken.None);
        }

        public void Play()
        {
            //_player.PlayAsync();
        }

        public void Stop()
        {
            _player.Stop(false);
        }

        public void Pause()
        {
            _player.Pause();
        }

        private void Log(string text)
        {
            Logging += $"{DateTime.Now} - {text}\r\n";
        }

        private async Task UpdateCounter()
        {
            CurrentTime = $@"{_player.CurrentTime:hh\:mm\:ss}";

            if (_player.PlaybackState == PlaybackState.Playing)
            {
                //trackBarSong.Value = (int)_player.CurrentTime.TotalSeconds;

                // Always select the listView to make sure the selected song is highlighted.
                //listView.Select();

                if (_player.CurrentTime.Add(TimeSpan.FromMilliseconds(500)) > _player.TotalTime)
                {
                    await _player.PlayNextAsync(CancellationToken.None);
                }
            }

            StateHasChanged();
        }

        private async Task RefreshTreeAsync()
        {
            _player.Stop(true);
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
    }
}
