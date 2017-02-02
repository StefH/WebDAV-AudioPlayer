using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace WebDav.AudioPlayer.UI
{
    public partial class DownloadFolderForm : Form
    {
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public DownloadFolderForm()
        {
            InitializeComponent();
        }

        private void btnAbortFolderDownload_Click(object sender, EventArgs e)
        {
            CancellationTokenSource.Cancel();
        }
    }
}
