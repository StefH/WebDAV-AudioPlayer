using System;
using System.Windows.Forms;
using WebDav.AudioPlayer.Audio;
using WebDav.AudioPlayer.UI;

namespace WebDav.AudioPlayer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var config = AssemblyConfig.Load();

            bool doConfig = false;
            if (config.IsDefault)
            {
                if (MessageBox.Show(@"You have not configured your WebDAV account yet.", @"Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    return;

                doConfig = true;
            }

            if (doConfig && new ConfigurationForm(config).ShowDialog() != DialogResult.OK)
                return;

            string version;
            if (!MediaInfoHelper.TryGetVersion(out version))
            {
                MessageBox.Show(@"MediaInfo.dll was not found or could not be loaded.", @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new MainForm(config));
        }
    }
}