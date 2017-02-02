namespace WebDav.AudioPlayer.UI
{
    partial class DownloadFolderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadFolderForm));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblDownloadFolderProgress = new System.Windows.Forms.Label();
            this.btnAbortFolderDownload = new System.Windows.Forms.Button();
            this.lblPct = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(23, 30);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 0;
            this.progressBar1.UseWaitCursor = true;
            // 
            // lblDownloadFolderProgress
            // 
            this.lblDownloadFolderProgress.AutoSize = true;
            this.lblDownloadFolderProgress.Location = new System.Drawing.Point(20, 9);
            this.lblDownloadFolderProgress.Name = "lblDownloadFolderProgress";
            this.lblDownloadFolderProgress.Size = new System.Drawing.Size(131, 13);
            this.lblDownloadFolderProgress.TabIndex = 1;
            this.lblDownloadFolderProgress.Text = "Folder download progress:";
            // 
            // btnAbortFolderDownload
            // 
            this.btnAbortFolderDownload.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAbortFolderDownload.Location = new System.Drawing.Point(198, 67);
            this.btnAbortFolderDownload.Name = "btnAbortFolderDownload";
            this.btnAbortFolderDownload.Size = new System.Drawing.Size(75, 23);
            this.btnAbortFolderDownload.TabIndex = 2;
            this.btnAbortFolderDownload.Text = "Cancel";
            this.btnAbortFolderDownload.UseVisualStyleBackColor = true;
            this.btnAbortFolderDownload.Click += new System.EventHandler(this.btnAbortFolderDownload_Click);
            // 
            // lblPct
            // 
            this.lblPct.AutoSize = true;
            this.lblPct.Location = new System.Drawing.Point(151, 9);
            this.lblPct.Name = "lblPct";
            this.lblPct.Size = new System.Drawing.Size(21, 13);
            this.lblPct.TabIndex = 3;
            this.lblPct.Text = "0%";
            // 
            // DownloadFolderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CancelButton = this.btnAbortFolderDownload;
            this.ClientSize = new System.Drawing.Size(300, 102);
            this.ControlBox = false;
            this.Controls.Add(this.lblPct);
            this.Controls.Add(this.btnAbortFolderDownload);
            this.Controls.Add(this.lblDownloadFolderProgress);
            this.Controls.Add(this.progressBar1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DownloadFolderForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "DownloadFolderForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblDownloadFolderProgress;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnAbortFolderDownload;
        public System.Windows.Forms.Label lblPct;
    }
}