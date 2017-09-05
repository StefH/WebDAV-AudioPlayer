using System;
using System.Threading;
using System.Windows.Forms;

namespace WebDav.AudioPlayer.UI
{
    public partial class DownloadResourceItemForm : Form
    {
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public DownloadResourceItemForm()
        {
            InitializeComponent();
        }

        private void btnAbortFolderDownload_Click(object sender, EventArgs e)
        {
            CancellationTokenSource.Cancel();
        }
    }
}