using System;
using System.Text;
using System.Windows.Forms;
using WebDav.AudioPlayer.Properties;

namespace WebDav.AudioPlayer.UI
{
    public partial class ConfigurationForm : Form
    {
        private readonly AssemblyConfig _config;

        public ConfigurationForm(AssemblyConfig config)
        {
            InitializeComponent();

            Icon = Resources.icon;
            _config = config;

            if (!_config.IsDefault)
            {
                txtUri.Text = _config.StorageUri.ToString();
                txtUserName.Text = _config.UserName;
                txtPassword.Text = _config.GetPassword();
                txtRootFolder.Text = config.RootFolder;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var errorMessage = new StringBuilder();
            Uri uri;
            if (!Uri.TryCreate(txtUri.Text, UriKind.Absolute, out uri) || uri == null)
                errorMessage.Append("\r\n- The WebDAV url is empty or not valid");
            else if (uri.Scheme != "https")
                errorMessage.Append("\r\n- The WebDAV url should be HTTPS");
            if (string.IsNullOrEmpty(txtUserName.Text))
                errorMessage.Append("\r\n- The user name cannot be empty");
            if (string.IsNullOrEmpty(txtPassword.Text))
                errorMessage.Append("\r\n- The password cannot be empty");

            if (errorMessage.Length > 0)
            {
                MessageBox.Show(@"Cannot save the settings because of the following issues:" + errorMessage, @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _config.StorageUri = uri;
            _config.UserName = txtUserName.Text;
            _config.SetPassword(txtPassword.Text);
            _config.RootFolder = string.IsNullOrWhiteSpace(txtRootFolder.Text) ? null : txtRootFolder.Text;
            _config.Save();

            DialogResult = DialogResult.OK;
        }
    }
}