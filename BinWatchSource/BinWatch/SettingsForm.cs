using System;
using System.IO;
using System.Windows.Forms;

namespace BinWatch
{
    public partial class SettingsForm : Form
    {
        // The DB path currently in use by the running app
        private readonly string _currentDbPath;

        public SettingsForm()
        {
            InitializeComponent();

            _currentDbPath         = AppConfig.ResolvedDbPath;
            txtDbPath.Text         = AppConfig.DbPath;
            chkPassiveMode.Checked = AppConfig.PassiveMode;

            UpdateCopyDbVisibility();
        }

        private void txtDbPath_TextChanged(object sender, EventArgs e)
        {
            UpdateCopyDbVisibility();
        }

        // Show the copy checkbox only when the user has entered a different path
        // and the current DB file actually exists to copy from.
        private void UpdateCopyDbVisibility()
        {
            string newPath = txtDbPath.Text.Trim();
            bool differentPath = !string.IsNullOrWhiteSpace(newPath)
                && !string.Equals(newPath, _currentDbPath, StringComparison.OrdinalIgnoreCase);
            bool sourceExists = File.Exists(_currentDbPath);

            chkCopyDb.Visible = differentPath && sourceExists;
            if (chkCopyDb.Visible && !chkCopyDb.Checked)
                chkCopyDb.Checked = true;  // default to checked when it first appears
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title           = "Select or create database file";
                dlg.Filter          = "SQLite database (*.db)|*.db";
                dlg.FileName        = "BinWatch.db";
                dlg.OverwritePrompt = false;  // allow selecting an existing file

                string current = txtDbPath.Text.Trim();
                if (!string.IsNullOrEmpty(current))
                {
                    try { dlg.InitialDirectory = Path.GetDirectoryName(current); }
                    catch { }
                }

                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtDbPath.Text = dlg.FileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string newPath = txtDbPath.Text.Trim();

            bool copyDb = chkCopyDb.Visible && chkCopyDb.Checked;

            // Write to file only — do NOT update in-memory AppConfig properties.
            // The running app keeps using its current DB path and mode until restarted.
            AppConfig.Save(
                dbPath:        newPath,
                passiveMode:   chkPassiveMode.Checked,
                copyDbOnStart: copyDb,
                copyDbSource:  copyDb ? _currentDbPath : "");

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
