using Blazor.WebDAV.AudioPlayer.Client;
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
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace Blazor.WebDAV.AudioPlayer.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        protected IOptions<ConnectionSettings> Options { get; set; }

        [Inject]
        protected IWebDavClientFactory Factory { get; set; }

        protected TreeNode<ResourceItem> Root { get; private set; }

        protected List<PlayListItem> PlayListItems { get; private set; } = new List<PlayListItem>();

        protected ResourceLoadStatus Status { get; private set; }

        private IWebDavClient _client;

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();

            _client = Factory.GetClient();

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
            //PlayListItems.Clear();

            if (treeNode.Item.Items == null)
            {
                Status = await _client.FetchChildResourcesAsync(treeNode.Item, CancellationToken.None, treeNode.Item.Level, treeNode.Item.Level);
            }

            if (Status == ResourceLoadStatus.Ok)
            {
                PlayListItems = treeNode.Item.Items
                    .Where(resourceItem => !resourceItem.IsCollection)
                    .Select(resourceItem => new PlayListItem
                    {
                        Item = resourceItem,
                        Title = resourceItem.DisplayName,
                        Bitrate = null, // resourceItem.MediaDetails.BitrateKbps,
                        Size = resourceItem.ContentLength != null ? ByteSize.FromBytes(resourceItem.ContentLength.Value).ToString("0.00 MB") : string.Empty,
                        Length = null //TimeSpan.FromMilliseconds(resourceItem.MediaDetails.DurationMs).ToString("h:mm:ss")
                    }).ToList();

                int x = 9;
            }

            //if (treeNode.Item.Items == null)
            //{
            //    Status = await _client.FetchChildResourcesAsync(treeNode.Item, CancellationToken.None, treeNode.Item.Level, treeNode.Item.Level);
            //}

            //if (Status == ResourceLoadStatus.Ok)
            //{
            //    //node.Nodes.Clear();
            //    //PopulateTree(ref node, resourceItem.Items);
            //    //node.Expand();

            //    //_player.Items = resourceItem.Items.Where(r => !r.IsCollection).ToList();

            //    //listView.Items.Clear();
            //    //foreach (var file in _player.Items)
            //    //{
            //    //    string size = file.ContentLength != null ? ByteSize.FromBytes(file.ContentLength.Value).ToString("0.00 MB") : string.Empty;
            //    //    var listViewItem = new ListViewItem(new[] { file.DisplayName, size, null, null }) { Tag = file };
            //    //    listView.Items.Add(listViewItem);
            //    //}

            //    var musicFiles = treeNode.Item.Items.Where(i => !i.IsCollection).Select(resourceItem => new TreeNode<ResourceItem>
            //    {
            //        Item = resourceItem,
            //        Title = resourceItem.DisplayName,
            //        Bitrate = resourceItem.MediaDetails.BitrateKbps,
            //        Size = resourceItem.ContentLength != null ? ByteSize.FromBytes(resourceItem.ContentLength.Value).ToString("0.00 MB") : string.Empty,
            //        Length = TimeSpan.FromMilliseconds(resourceItem.MediaDetails.DurationMs).ToString("h:mm:ss")
            //    });
            //}
        }

        private async Task RefreshTreeAsync()
        {
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
