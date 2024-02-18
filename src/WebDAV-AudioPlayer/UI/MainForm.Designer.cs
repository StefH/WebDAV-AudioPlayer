namespace WebDav.AudioPlayer.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.textBoxSong = new System.Windows.Forms.TextBox();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageListTreeView = new System.Windows.Forms.ImageList(this.components);
            this.toolStripTreeView = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.panelRightMid = new System.Windows.Forms.Panel();
            this.listView = new System.Windows.Forms.ListView();
            this.DisplayName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ContentLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Length = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Bitrate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelRightBottom = new System.Windows.Forms.Panel();
            this.txtLogging = new System.Windows.Forms.TextBox();
            this.panelRightTop = new System.Windows.Forms.Panel();
            this.toolStripRight = new System.Windows.Forms.ToolStrip();
            this.buttonPlay = new System.Windows.Forms.ToolStripButton();
            this.buttonStop = new System.Windows.Forms.ToolStripButton();
            this.buttonPause = new System.Windows.Forms.ToolStripButton();
            this.btnPrevious = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.btnSettings = new System.Windows.Forms.ToolStripButton();
            this.tsLblCurrentTime = new System.Windows.Forms.ToolStripLabel();
            this.labelCurrentTime = new System.Windows.Forms.ToolStripLabel();
            this.tsLblTotalTime = new System.Windows.Forms.ToolStripLabel();
            this.labelTotalTime = new System.Windows.Forms.ToolStripLabel();
            this.trackBarSong = new System.Windows.Forms.TrackBar();
            this.audioPlaybackTimer = new System.Windows.Forms.Timer(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.contextMenuStripOnFolder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.save = new System.Windows.Forms.ToolStripMenuItem();
            this.cancel = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolStripTreeView.SuspendLayout();
            this.panelRightMid.SuspendLayout();
            this.panelRightBottom.SuspendLayout();
            this.panelRightTop.SuspendLayout();
            this.toolStripRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSong)).BeginInit();
            this.contextMenuStripOnFolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.textBoxSong);
            this.splitContainer.Panel1.Controls.Add(this.treeView);
            this.splitContainer.Panel1.Controls.Add(this.toolStripTreeView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panelRightMid);
            this.splitContainer.Panel2.Controls.Add(this.panelRightBottom);
            this.splitContainer.Panel2.Controls.Add(this.panelRightTop);
            this.splitContainer.Size = new System.Drawing.Size(1476, 1032);
            this.splitContainer.SplitterDistance = 727;
            this.splitContainer.SplitterWidth = 6;
            this.splitContainer.TabIndex = 1;
            // 
            // textBoxSong
            // 
            this.textBoxSong.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSong.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBoxSong.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSong.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.textBoxSong.Location = new System.Drawing.Point(0, 1006);
            this.textBoxSong.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxSong.Name = "textBoxSong";
            this.textBoxSong.ReadOnly = true;
            this.textBoxSong.Size = new System.Drawing.Size(727, 26);
            this.textBoxSong.TabIndex = 2;
            // 
            // treeView
            // 
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageListTreeView;
            this.treeView.Location = new System.Drawing.Point(0, 33);
            this.treeView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(727, 999);
            this.treeView.TabIndex = 0;
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // imageListTreeView
            // 
            this.imageListTreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTreeView.ImageStream")));
            this.imageListTreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTreeView.Images.SetKeyName(0, "Drive");
            this.imageListTreeView.Images.SetKeyName(1, "Folder");
            this.imageListTreeView.Images.SetKeyName(2, "File");
            this.imageListTreeView.Images.SetKeyName(3, "icon");
            // 
            // toolStripTreeView
            // 
            this.toolStripTreeView.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh});
            this.toolStripTreeView.Location = new System.Drawing.Point(0, 0);
            this.toolStripTreeView.Name = "toolStripTreeView";
            this.toolStripTreeView.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStripTreeView.Size = new System.Drawing.Size(727, 33);
            this.toolStripTreeView.TabIndex = 1;
            this.toolStripTreeView.Text = "toolStripTreeView";
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::WebDav.AudioPlayer.Properties.Resources.Refresh;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(34, 28);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // panelRightMid
            // 
            this.panelRightMid.BackColor = System.Drawing.SystemColors.Control;
            this.panelRightMid.Controls.Add(this.listView);
            this.panelRightMid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRightMid.Location = new System.Drawing.Point(0, 123);
            this.panelRightMid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelRightMid.Name = "panelRightMid";
            this.panelRightMid.Size = new System.Drawing.Size(743, 801);
            this.panelRightMid.TabIndex = 2;
            // 
            // listView
            // 
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DisplayName,
            this.ContentLength,
            this.Length,
            this.Bitrate});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.GridLines = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Margin = new System.Windows.Forms.Padding(0);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(743, 801);
            this.listView.TabIndex = 17;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            this.listView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseDoubleClick);
            // 
            // DisplayName
            // 
            this.DisplayName.Tag = "DisplayName";
            this.DisplayName.Text = "Title";
            this.DisplayName.Width = 500;
            // 
            // ContentLength
            // 
            this.ContentLength.Tag = "ContentLength";
            this.ContentLength.Text = "Size";
            this.ContentLength.Width = 100;
            // 
            // Length
            // 
            this.Length.Tag = "Length";
            this.Length.Text = "Length";
            this.Length.Width = 100;
            // 
            // Bitrate
            // 
            this.Bitrate.Tag = "BitrateKbps";
            this.Bitrate.Text = "BitrateKbps";
            this.Bitrate.Width = 100;
            // 
            // panelRightBottom
            // 
            this.panelRightBottom.BackColor = System.Drawing.SystemColors.Control;
            this.panelRightBottom.Controls.Add(this.txtLogging);
            this.panelRightBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelRightBottom.Location = new System.Drawing.Point(0, 924);
            this.panelRightBottom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelRightBottom.Name = "panelRightBottom";
            this.panelRightBottom.Size = new System.Drawing.Size(743, 108);
            this.panelRightBottom.TabIndex = 1;
            // 
            // txtLogging
            // 
            this.txtLogging.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLogging.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogging.ForeColor = System.Drawing.Color.Green;
            this.txtLogging.Location = new System.Drawing.Point(0, 0);
            this.txtLogging.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtLogging.Multiline = true;
            this.txtLogging.Name = "txtLogging";
            this.txtLogging.ReadOnly = true;
            this.txtLogging.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogging.Size = new System.Drawing.Size(743, 108);
            this.txtLogging.TabIndex = 18;
            // 
            // panelRightTop
            // 
            this.panelRightTop.BackColor = System.Drawing.SystemColors.Control;
            this.panelRightTop.Controls.Add(this.toolStripRight);
            this.panelRightTop.Controls.Add(this.trackBarSong);
            this.panelRightTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRightTop.Location = new System.Drawing.Point(0, 0);
            this.panelRightTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelRightTop.Name = "panelRightTop";
            this.panelRightTop.Size = new System.Drawing.Size(743, 123);
            this.panelRightTop.TabIndex = 0;
            // 
            // toolStripRight
            // 
            this.toolStripRight.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripRight.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonPlay,
            this.buttonStop,
            this.buttonPause,
            this.btnPrevious,
            this.btnNext,
            this.btnSettings,
            this.tsLblCurrentTime,
            this.labelCurrentTime,
            this.tsLblTotalTime,
            this.labelTotalTime});
            this.toolStripRight.Location = new System.Drawing.Point(0, 0);
            this.toolStripRight.Name = "toolStripRight";
            this.toolStripRight.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStripRight.Size = new System.Drawing.Size(743, 33);
            this.toolStripRight.TabIndex = 16;
            this.toolStripRight.Text = "toolStripRight";
            // 
            // buttonPlay
            // 
            this.buttonPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPlay.Image = ((System.Drawing.Image)(resources.GetObject("buttonPlay.Image")));
            this.buttonPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(34, 28);
            this.buttonPlay.Text = "Play";
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonStop.Image = ((System.Drawing.Image)(resources.GetObject("buttonStop.Image")));
            this.buttonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(34, 28);
            this.buttonStop.Text = "Stop";
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPause.Image = ((System.Drawing.Image)(resources.GetObject("buttonPause.Image")));
            this.buttonPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(34, 28);
            this.buttonPause.Text = "Pause";
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrevious.Image = global::WebDav.AudioPlayer.Properties.Resources.Previous;
            this.btnPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(34, 28);
            this.btnPrevious.Text = "Previous";
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnNext
            // 
            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNext.Image = global::WebDav.AudioPlayer.Properties.Resources.Next;
            this.btnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(34, 28);
            this.btnNext.Text = "Next";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSettings.Image = global::WebDav.AudioPlayer.Properties.Resources.Settings;
            this.btnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(34, 28);
            this.btnSettings.Text = "Settings";
            // 
            // tsLblCurrentTime
            // 
            this.tsLblCurrentTime.Name = "tsLblCurrentTime";
            this.tsLblCurrentTime.Size = new System.Drawing.Size(79, 28);
            this.tsLblCurrentTime.Text = "Current :";
            // 
            // labelCurrentTime
            // 
            this.labelCurrentTime.Name = "labelCurrentTime";
            this.labelCurrentTime.Size = new System.Drawing.Size(80, 28);
            this.labelCurrentTime.Text = "00:00:00";
            // 
            // tsLblTotalTime
            // 
            this.tsLblTotalTime.Name = "tsLblTotalTime";
            this.tsLblTotalTime.Size = new System.Drawing.Size(58, 28);
            this.tsLblTotalTime.Text = "Total :";
            // 
            // labelTotalTime
            // 
            this.labelTotalTime.Name = "labelTotalTime";
            this.labelTotalTime.Size = new System.Drawing.Size(80, 28);
            this.labelTotalTime.Text = "00:00:00";
            // 
            // trackBarSong
            // 
            this.trackBarSong.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarSong.LargeChange = 10;
            this.trackBarSong.Location = new System.Drawing.Point(4, 45);
            this.trackBarSong.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.trackBarSong.Name = "trackBarSong";
            this.trackBarSong.Size = new System.Drawing.Size(735, 69);
            this.trackBarSong.SmallChange = 5;
            this.trackBarSong.TabIndex = 0;
            this.trackBarSong.TickFrequency = 5;
            this.trackBarSong.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarSong.Scroll += new System.EventHandler(this.trackBarSong_Scroll);
            this.trackBarSong.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBarSong_MouseDown);
            this.trackBarSong.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarSong_MouseUp);
            // 
            // audioPlaybackTimer
            // 
            this.audioPlaybackTimer.Enabled = true;
            this.audioPlaybackTimer.Interval = 249;
            this.audioPlaybackTimer.Tick += new System.EventHandler(this.audioPlaybackTimer_Tick);
            // 
            // contextMenuStripOnFolder
            // 
            this.contextMenuStripOnFolder.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStripOnFolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.save,
            this.cancel});
            this.contextMenuStripOnFolder.Name = "contextMenuStripOnFolder";
            this.contextMenuStripOnFolder.Size = new System.Drawing.Size(199, 68);
            this.contextMenuStripOnFolder.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStripOnFolder_ItemClicked);
            // 
            // save
            // 
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(198, 32);
            this.save.Tag = "save";
            this.save.Text = "&Save to Folder";
            // 
            // cancel
            // 
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(198, 32);
            this.cancel.Tag = "cancel";
            this.cancel.Text = "&Cancel";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1476, 1032);
            this.Controls.Add(this.splitContainer);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "WebDAV-AudioPlayer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.toolStripTreeView.ResumeLayout(false);
            this.toolStripTreeView.PerformLayout();
            this.panelRightMid.ResumeLayout(false);
            this.panelRightBottom.ResumeLayout(false);
            this.panelRightBottom.PerformLayout();
            this.panelRightTop.ResumeLayout(false);
            this.panelRightTop.PerformLayout();
            this.toolStripRight.ResumeLayout(false);
            this.toolStripRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSong)).EndInit();
            this.contextMenuStripOnFolder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageListTreeView;
        private System.Windows.Forms.ToolStrip toolStripRight;
        private System.Windows.Forms.ToolStripButton buttonPlay;
        private System.Windows.Forms.ToolStripButton buttonPause;
        private System.Windows.Forms.ToolStripButton buttonStop;
        private System.Windows.Forms.ToolStripLabel tsLblCurrentTime;
        private System.Windows.Forms.ToolStripLabel labelCurrentTime;
        private System.Windows.Forms.ToolStripLabel tsLblTotalTime;
        private System.Windows.Forms.ToolStripLabel labelTotalTime;
        private System.Windows.Forms.ToolStripButton btnNext;
        private System.Windows.Forms.ToolStripButton btnPrevious;
        private System.Windows.Forms.ToolStrip toolStripTreeView;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader DisplayName;
        private System.Windows.Forms.Timer audioPlaybackTimer;
        private System.Windows.Forms.ToolStripButton btnSettings;
        private System.Windows.Forms.TextBox textBoxSong;
        private System.Windows.Forms.TextBox txtLogging;
        private System.Windows.Forms.ColumnHeader ContentLength;
        private System.Windows.Forms.TrackBar trackBarSong;
        private System.Windows.Forms.Panel panelRightTop;
        private System.Windows.Forms.Panel panelRightBottom;
        private System.Windows.Forms.Panel panelRightMid;
        private System.Windows.Forms.ColumnHeader Bitrate;
        private System.Windows.Forms.ColumnHeader Length;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOnFolder;
        private System.Windows.Forms.ToolStripMenuItem save;
        private System.Windows.Forms.ToolStripMenuItem cancel;
    }
}