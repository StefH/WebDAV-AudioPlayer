using ByteSizeLib;
using CSCore.SoundOut;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;
using WebDav.AudioPlayer.Properties;

namespace WebDav.AudioPlayer.UI
{
    public partial class MainForm : Form
    {
        private readonly AssemblyConfig _config;
        private readonly IWebDavClient _client;
        private readonly Player _player;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancelToken;

        private float _scalingFactor; // = 1;
        private SizeF _scale; // = new SizeF(1, 1);

        public MainForm(AssemblyConfig config)
        {
            _scalingFactor = DeviceDpi / UIConstants.StandardDPI;
            _scale = new SizeF(_scalingFactor, _scalingFactor);

            InitializeComponent();

            _config = config;

            Icon = Resources.icon;

            InitCancellationTokenSource();

            _client = new MyWebDavClient(config);

            Func<ResourceItem, string, string> updateTitle = (resourceItem, action) =>
            {
                var text = $"{action} : '{resourceItem.Parent.DisplayName}\\{resourceItem.DisplayName}' ({resourceItem.MediaDetails.Mode} {resourceItem.MediaDetails.BitrateKbps} kbps)";
                Text = @"WebDAV-AudioPlayer " + text;

                return text;
            };

            _player = new Player(_client)
            {
                Log = Log,

                PlayStarted = (player, selectedIndex, resourceItem) =>
                {
                    var bitrateAsString = $"{resourceItem.MediaDetails.BitrateKbps}";
                    var text = updateTitle(resourceItem, "Playing");
                    textBoxSong.Text = text;

                    labelTotalTime.Text = $@"{player.TotalTime:hh\:mm\:ss}";

                    trackBarSong.Maximum = (int)player.TotalTime.TotalSeconds;
                    trackBarSong.Enabled = player.CanSeek;

                    listView.SetCells(selectedIndex, $@"{player.TotalTime:h\:mm\:ss}", bitrateAsString);
                    listView.SetSelectedIndex(selectedIndex);
                },
                PlayContinue = resourceItem =>
                {
                    var text = updateTitle(resourceItem, "Playing");
                    textBoxSong.Text = text;
                },
                PlayPaused = resourceItem =>
                {
                    var text = updateTitle(resourceItem, "Pausing");
                    textBoxSong.Text = text;
                },
                PlayStopped = () =>
                {
                    trackBarSong.Value = 0;
                    trackBarSong.Maximum = 1;
                    labelCurrentTime.Text = labelTotalTime.Text = @"00:00:00";
                    Text = @"WebDAV-AudioPlayer";
                },
                DoubleClickFolderAndPlayFirstSong = async resourceItem =>
                {
                    var nodeToDoubleClick = treeView.Nodes.Find(resourceItem.DisplayName, true).FirstOrDefault();
                    if (nodeToDoubleClick != null)
                    {
                        await FetchChildResourcesAsync(nodeToDoubleClick, resourceItem);
                    }
                }
            };

            Log($"Using : '{_player.SoundOut}-SoundOut'");
        }

        //protected override void OnDpiChanged(DpiChangedEventArgs e)
        //{
        //    _scalingFactor = DeviceDpi / UIConstants.StandardDPI;
        //    _scale = new SizeF(_scalingFactor, _scalingFactor);

        //    Scale();

        //    base.OnDpiChanged(e);
        //}

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            Scale();

            base.ScaleControl(_scale, specified);
        }

        private void Scale()
        {
            Font = new Font(UIConstants.FontFamilyName, UIConstants.FontSize * _scalingFactor);

            foreach (ColumnHeader column in listView.Columns)
            {
                column.Width = (int)Math.Round(column.Width * _scalingFactor);
            }

            textBoxSong.Font = Font;

            toolStripRight.Font = Font;
            toolStripRight.Scale(_scale);
            foreach (var toolStripRightItem in toolStripRight.Items.OfType<ToolStripItem>())
            {
                // toolStripRightItem.Font = Font;
                if (toolStripRightItem is ToolStripButton toolStripButton)
                {
                    // toolStripButton.Size = new Size((int)(toolStripButton.Width * _scalingFactor), (int)(toolStripButton.Height * _scalingFactor));
                    // toolStripButton.Image = ResizeImage(toolStripButton.Image);
                }
            }
        }

        //private Image ResizeImage(Image originalImage)
        //{
        //    int width = (int)(originalImage.Width * _scalingFactor);
        //    int height = (int)(originalImage.Height * _scalingFactor);

        //    var resizedImage = new Bitmap(width, height);
        //    using (var graphics = Graphics.FromImage(resizedImage))
        //    {
        //        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        //        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //        graphics.DrawImage(originalImage, 0, 0, width, height);
        //    }

        //    return resizedImage;
        //}

        private void InitCancellationTokenSource()
        {
            _cancellationTokenSource?.Cancel();

            _cancellationTokenSource = new CancellationTokenSource();
            _cancelToken = _cancellationTokenSource.Token;
        }

        private void Log(string text)
        {
            txtLogging.AppendText($"{DateTime.Now} - {text}\r\n");
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await RefreshTreeAsync().ConfigureAwait(false);
        }

        private async Task RefreshTreeAsync()
        {
            var current = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            treeView.Nodes.Clear();

            var root = new ResourceItem
            {
                DisplayName = _config.RootFolder,
                FullPath = OnlinePathBuilder.ConvertPathToFullUri(_config.StorageUri, _config.RootFolder)
            };

            var result = await _client.FetchChildResourcesAsync(root, _cancelToken, 0);
            if (result != ResourceLoadStatus.Ok)
            {
                return;
            }

            var rootNode = new TreeNode
            {
                Text = _config.RootFolder,
                Tag = null
            };

            PopulateTree(ref rootNode, root.Items);

            treeView.Nodes.Add(rootNode);
            rootNode.Expand();

            Cursor.Current = current;
        }

        private void PopulateTree(ref TreeNode node, IList<ResourceItem> resourceItems)
        {
            if (resourceItems == null)
            {
                return;
            }

            if (node == null)
            {
                node = new TreeNode
                {
                    Text = _config.RootFolder,
                    Tag = null
                };
            }

            foreach (var resourceItem in resourceItems.Where(r => r.IsCollection))
            {
                var childNode = new TreeNode
                {
                    Text = resourceItem.DisplayName,
                    Name = resourceItem.DisplayName,
                    Tag = resourceItem,
                    ImageKey = @"Folder",
                    SelectedImageKey = @"Folder",
                    ContextMenuStrip = contextMenuStripOnFolder
                };

                PopulateTree(ref childNode, resourceItem.Items);
                node.Nodes.Add(childNode);
            }
        }

        private async void contextMenuStripOnFolder_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag is ResourceItem resourceItem && e.ClickedItem.Name == "save")
            {
                await ShowFolderSaveDialogAsync(resourceItem);
            }
        }

        private async Task ShowFolderSaveDialogAsync(ResourceItem resourceItem)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                var progress = CreateAndShowDownloadFolderForm();

                Action<bool, ResourceItem, int, int> notify = (success, item, index, total) =>
                {
                    int pct = (int)(100.0 * index / total);
                    progress.lblPct.Text = $@"{pct}%";
                    progress.progressBar1.Value = pct;
                };

                ResourceLoadStatus status = ResourceLoadStatus.Unknown;
                try
                {
                    status = await _client.DownloadFolderAsync(resourceItem, folderBrowserDialog1.SelectedPath, notify, _cancelToken);

                    progress.lblPct.Text = "100%";
                    progress.progressBar1.Value = 100;

                    await Task.Delay(TimeSpan.FromMilliseconds(500), _cancelToken);
                }
                catch // (Exception e)
                {
                    // MessageBox.Show(e.Message, "Error saving folder.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                finally
                {
                    Log(status == ResourceLoadStatus.Ok
                        ? $"Folder '{resourceItem.DisplayName}' saved to '{folderBrowserDialog1.SelectedPath}'"
                        : $"Folder '{resourceItem.DisplayName}' was not saved correctly : {status}");

                    progress.Close();
                    InitCancellationTokenSource();
                }
            }
        }

        private DownloadFolderForm CreateAndShowDownloadFolderForm()
        {
            var progress = new DownloadFolderForm
            {
                Owner = this,
                CancellationTokenSource = _cancellationTokenSource,
                StartPosition = FormStartPosition.Manual
            };
            progress.Location = new Point(Location.X + (Width - progress.Width) / 2, Location.Y + (Height - progress.Height) / 2);
            progress.Show();
            return progress;
        }

        private async void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var hitTest = e.Node.TreeView.HitTest(e.Location);
            if (hitTest.Location == TreeViewHitTestLocations.PlusMinus)
            {
                return;
            }

            var resourceItem = e.Node.Tag as ResourceItem;
            if (resourceItem == null)
            {
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                contextMenuStripOnFolder.Items["save"].Tag = resourceItem;
                contextMenuStripOnFolder.Show(Cursor.Position);
                return;
            }

            var current = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            await FetchChildResourcesAsync(e.Node, resourceItem);

            Cursor.Current = current;
        }

        private async Task FetchChildResourcesAsync(TreeNode node, ResourceItem resourceItem)
        {
            var result = await _client.FetchChildResourcesAsync(resourceItem, _cancelToken, resourceItem.Level, resourceItem.Level);
            if (result == ResourceLoadStatus.Ok)
            {
                node.Nodes.Clear();
                PopulateTree(ref node, resourceItem.Items);
                node.Expand();

                _player.Items = resourceItem.Items.Where(r => !r.IsCollection).ToList();

                listView.Items.Clear();
                foreach (var file in _player.Items)
                {
                    string size = file.ContentLength != null ? ByteSize.FromBytes(file.ContentLength.Value).ToString("0.00 MB") : string.Empty;
                    var listViewItem = new ListViewItem(new[] { file.DisplayName, size, null, null }) { Tag = file };
                    listView.Items.Add(listViewItem);
                }
            }
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _player.PlayAsync(listView.SelectedIndices[0], _cancelToken);
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (listView.Items.Count == 0)
            {
                return;
            }

            switch (e.KeyData)
            {
                case Keys.Enter:
                    _player.PlayAsync(listView.SelectedIndices[0], _cancelToken);
                    break;

                case Keys.PageUp:
                    listView.SetSelectedIndex(0);
                    break;

                case Keys.PageDown:
                    listView.SetSelectedIndex(listView.Items.Count - 1);
                    break;

                case Keys.Up:
                    int upIndex = listView.SelectedIndices[0] - 1;
                    if (upIndex > 0)
                    {
                        listView.SetSelectedIndex(upIndex);
                    }
                    break;

                case Keys.Down:

                    int downIndex = listView.SelectedIndices[0] + 1;
                    if (downIndex < listView.Items.Count)
                    {
                        listView.SetSelectedIndex(downIndex);
                    }
                    break;
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            _player.PlayAsync(listView.SelectedIndices[0], _cancelToken);
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            _player.Pause();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _player.Stop(true);
            InitCancellationTokenSource();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            _player.PlayPreviousAsync(_cancelToken);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _player.PlayNextAsync(_cancelToken);
        }

        private void trackBarSong_Scroll(object sender, EventArgs e)
        {
            _player.JumpTo(TimeSpan.FromSeconds(trackBarSong.Value));
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshTreeAsync().ConfigureAwait(false);
        }

        private void trackBarSong_MouseDown(object sender, MouseEventArgs e)
        {
            _player.SetVolume(0);
        }

        private void trackBarSong_MouseUp(object sender, MouseEventArgs e)
        {
            _player.SetVolume(1);
        }

        private void audioPlaybackTimer_Tick(object sender, EventArgs e)
        {
            if (_player != null)
            {
                labelCurrentTime.Text = $@"{_player.CurrentTime:hh\:mm\:ss}";

                if (_player.PlaybackState == PlaybackState.Playing)
                {
                    trackBarSong.Value = (int)_player.CurrentTime.TotalSeconds;
                    // trackBarSong.Enabled = _player.CanSeek;

                    // Always select the listView to make sure the selected song is highlighted.
                    listView.Select();

                    if (_player.CurrentTime.Add(TimeSpan.FromMilliseconds(500)) > _player.TotalTime)
                    {
                        _player.PlayNextAsync(_cancelToken);
                    }
                }
            }
        }

        private static void ScaleListViewColumns(ListView lv, SizeF factor)
        {
            foreach (ColumnHeader column in lv.Columns)
            {
                column.Width = (int)Math.Round(column.Width * factor.Width);
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            _player.Dispose();

            _client.Dispose();

            base.Dispose(disposing);
        }
    }
}