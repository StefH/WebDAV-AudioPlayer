using Blazor.WebDAV.AudioPlayer.Client;
using Blazor.WebDAV.AudioPlayer.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
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

        protected ResourceItem Root { get; private set; }

        protected ResourceLoadStatus Status { get; private set; }

        private IWebDavClient _client;

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();

            _client = Factory.GetClient();

            Root = new ResourceItem
            {
                DisplayName = Options.Value.RootFolder,
                FullPath = OnlinePathBuilder.Combine(Options.Value.StorageUri, Options.Value.RootFolder)
            };

            await RefreshTreeAsync();
        }

        public async Task Refresh()
        {
            await RefreshTreeAsync();
        }

        protected async Task SelectedResourceItemChangedAsync(ResourceItem resourceItem)
        {
            Status = await _client.FetchChildResourcesAsync(resourceItem, CancellationToken.None, resourceItem.Level, resourceItem.Level);
            if (Status == ResourceLoadStatus.Ok)
            {
                //node.Nodes.Clear();
                //PopulateTree(ref node, resourceItem.Items);
                //node.Expand();

                //_player.Items = resourceItem.Items.Where(r => !r.IsCollection).ToList();

                //listView.Items.Clear();
                //foreach (var file in _player.Items)
                //{
                //    string size = file.ContentLength != null ? ByteSize.FromBytes(file.ContentLength.Value).ToString("0.00 MB") : string.Empty;
                //    var listViewItem = new ListViewItem(new[] { file.DisplayName, size, null, null }) { Tag = file };
                //    listView.Items.Add(listViewItem);
                //}
            }
        }

        private async Task RefreshTreeAsync()
        {
            Status = ResourceLoadStatus.Unknown;
            Status = await _client.FetchChildResourcesAsync(Root, CancellationToken.None, 0);
        }
    }
}
