namespace BinWatch
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblDbPath;
        private System.Windows.Forms.TextBox txtDbPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.CheckBox chkCopyDb;
        private System.Windows.Forms.CheckBox chkPassiveMode;
        private System.Windows.Forms.CheckBox chkDebugLogging;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components     = new System.ComponentModel.Container();
            this.lblDbPath      = new System.Windows.Forms.Label();
            this.txtDbPath      = new System.Windows.Forms.TextBox();
            this.btnBrowse      = new System.Windows.Forms.Button();
            this.chkCopyDb      = new System.Windows.Forms.CheckBox();
            this.chkPassiveMode  = new System.Windows.Forms.CheckBox();
            this.chkDebugLogging = new System.Windows.Forms.CheckBox();
            this.lblNote         = new System.Windows.Forms.Label();
            this.btnSave        = new System.Windows.Forms.Button();
            this.btnCancel      = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblDbPath
            this.lblDbPath.AutoSize = true;
            this.lblDbPath.Location = new System.Drawing.Point(12, 12);
            this.lblDbPath.Text     = "Database path:";

            // txtDbPath
            this.txtDbPath.Location  = new System.Drawing.Point(12, 32);
            this.txtDbPath.Size      = new System.Drawing.Size(520, 23);
            this.txtDbPath.Name      = "txtDbPath";
            this.txtDbPath.TextChanged += new System.EventHandler(this.txtDbPath_TextChanged);

            // btnBrowse
            this.btnBrowse.Location = new System.Drawing.Point(540, 31);
            this.btnBrowse.Size     = new System.Drawing.Size(65, 25);
            this.btnBrowse.Text     = "Browse…";
            this.btnBrowse.Click   += new System.EventHandler(this.btnBrowse_Click);

            // chkCopyDb  (shown/hidden in code depending on whether path changed)
            this.chkCopyDb.AutoSize = true;
            this.chkCopyDb.Location = new System.Drawing.Point(12, 62);
            this.chkCopyDb.Text     = "Copy current database to new location on next start";
            this.chkCopyDb.Name     = "chkCopyDb";
            this.chkCopyDb.Visible  = false;

            // chkPassiveMode
            this.chkPassiveMode.AutoSize = true;
            this.chkPassiveMode.Location = new System.Drawing.Point(12, 90);
            this.chkPassiveMode.Text     = "Passive mode  —  read shared database only, do not send UDP packets";
            this.chkPassiveMode.Name     = "chkPassiveMode";

            // chkDebugLogging
            this.chkDebugLogging.AutoSize = true;
            this.chkDebugLogging.Location = new System.Drawing.Point(12, 118);
            this.chkDebugLogging.Text     = "Debug logging  —  log individual packets and bad packet file";
            this.chkDebugLogging.Name     = "chkDebugLogging";

            // lblNote
            this.lblNote.AutoSize  = true;
            this.lblNote.ForeColor = System.Drawing.Color.Gray;
            this.lblNote.Location  = new System.Drawing.Point(12, 148);
            this.lblNote.Text      = "Database and mode changes require restart. Debug logging takes effect immediately.";

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(420, 178);
            this.btnSave.Size     = new System.Drawing.Size(90, 28);
            this.btnSave.Text     = "Save";
            this.btnSave.Click   += new System.EventHandler(this.btnSave_Click);

            // btnCancel
            this.btnCancel.Location     = new System.Drawing.Point(518, 178);
            this.btnCancel.Size         = new System.Drawing.Size(90, 28);
            this.btnCancel.Text         = "Cancel";
            this.btnCancel.DialogResult  = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Click        += new System.EventHandler(this.btnCancel_Click);

            // SettingsForm
            this.AcceptButton        = this.btnSave;
            this.CancelButton        = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Font                = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClientSize          = new System.Drawing.Size(622, 222);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblDbPath, txtDbPath, btnBrowse,
                chkCopyDb, chkPassiveMode, chkDebugLogging, lblNote,
                btnSave, btnCancel });
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.Name            = "SettingsForm";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text            = "Settings";
            this.ShowInTaskbar   = false;

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
