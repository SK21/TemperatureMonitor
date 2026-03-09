namespace BinTempsApp
{
    partial class SensorEditForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblRomCode;
        private System.Windows.Forms.Label lblRomCodeValue;
        private System.Windows.Forms.Label lblModule;
        private System.Windows.Forms.Label lblModuleValue;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label lblLocationValue;
        private System.Windows.Forms.Label lblLabel;
        private System.Windows.Forms.TextBox txtLabel;
        private System.Windows.Forms.Label lblMaxTemp;
        private System.Windows.Forms.NumericUpDown nudMaxTemp;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.NumericUpDown nudOffset;
        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblRomCode      = new System.Windows.Forms.Label();
            this.lblRomCodeValue = new System.Windows.Forms.Label();
            this.lblModule       = new System.Windows.Forms.Label();
            this.lblModuleValue  = new System.Windows.Forms.Label();
            this.lblLocation     = new System.Windows.Forms.Label();
            this.lblLocationValue = new System.Windows.Forms.Label();
            this.lblLabel        = new System.Windows.Forms.Label();
            this.txtLabel        = new System.Windows.Forms.TextBox();
            this.lblMaxTemp      = new System.Windows.Forms.Label();
            this.nudMaxTemp      = new System.Windows.Forms.NumericUpDown();
            this.lblOffset       = new System.Windows.Forms.Label();
            this.nudOffset       = new System.Windows.Forms.NumericUpDown();
            this.chkEnabled      = new System.Windows.Forms.CheckBox();
            this.btnSave         = new System.Windows.Forms.Button();
            this.btnCancel       = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)this.nudMaxTemp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.nudOffset).BeginInit();
            this.SuspendLayout();

            int col1 = 12, col2 = 130, rowH = 33;
            int row0 = 12, row1 = row0 + rowH, row2 = row1 + rowH,
                row3 = row2 + rowH, row4 = row3 + rowH,
                row5 = row4 + rowH, row6 = row5 + rowH, row7 = row6 + rowH;

            // ROM Code
            this.lblRomCode.AutoSize = true;
            this.lblRomCode.Location = new System.Drawing.Point(col1, row0 + 3);
            this.lblRomCode.Text     = "ROM Code:";
            this.lblRomCodeValue.AutoSize = true;
            this.lblRomCodeValue.Location = new System.Drawing.Point(col2, row0 + 3);
            this.lblRomCodeValue.Font     = new System.Drawing.Font("Consolas", 9F);

            // Module
            this.lblModule.AutoSize = true;
            this.lblModule.Location = new System.Drawing.Point(col1, row1 + 3);
            this.lblModule.Text     = "Module:";
            this.lblModuleValue.AutoSize = true;
            this.lblModuleValue.Location = new System.Drawing.Point(col2, row1 + 3);

            // Location
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(col1, row2 + 3);
            this.lblLocation.Text     = "Location:";
            this.lblLocationValue.AutoSize = true;
            this.lblLocationValue.Location = new System.Drawing.Point(col2, row2 + 3);

            // Label
            this.lblLabel.AutoSize = true;
            this.lblLabel.Location = new System.Drawing.Point(col1, row3 + 3);
            this.lblLabel.Text     = "Label:";
            this.txtLabel.Location  = new System.Drawing.Point(col2, row3);
            this.txtLabel.MaxLength = 30;
            this.txtLabel.Size      = new System.Drawing.Size(200, 23);

            // Max Temp
            this.lblMaxTemp.AutoSize = true;
            this.lblMaxTemp.Location = new System.Drawing.Point(col1, row4 + 3);
            this.lblMaxTemp.Text     = "Max Temp (°C):";
            this.nudMaxTemp.Location      = new System.Drawing.Point(col2, row4);
            this.nudMaxTemp.Minimum       = -50;
            this.nudMaxTemp.Maximum       = 100;
            this.nudMaxTemp.DecimalPlaces = 1;
            this.nudMaxTemp.Increment     = 0.5m;
            this.nudMaxTemp.Size          = new System.Drawing.Size(80, 23);

            // Offset
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(col1, row5 + 3);
            this.lblOffset.Text     = "Offset (°C):";
            this.nudOffset.Location      = new System.Drawing.Point(col2, row5);
            this.nudOffset.Minimum       = -10;
            this.nudOffset.Maximum       = 10;
            this.nudOffset.DecimalPlaces = 2;
            this.nudOffset.Increment     = 0.1m;
            this.nudOffset.Size          = new System.Drawing.Size(80, 23);

            // Enabled
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(col2, row6 + 3);
            this.chkEnabled.Text     = "Enabled";

            // Buttons
            this.btnSave.Location = new System.Drawing.Point(col2, row7);
            this.btnSave.Size     = new System.Drawing.Size(90, 28);
            this.btnSave.Text     = "Save";
            this.btnSave.Click   += new System.EventHandler(this.btnSave_Click);

            this.btnCancel.Location     = new System.Drawing.Point(col2 + 100, row7);
            this.btnCancel.Size         = new System.Drawing.Size(90, 28);
            this.btnCancel.Text         = "Cancel";
            this.btnCancel.DialogResult  = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Click        += new System.EventHandler(this.btnCancel_Click);

            // SensorEditForm
            this.AcceptButton      = this.btnSave;
            this.CancelButton      = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode     = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize        = new System.Drawing.Size(360, row7 + 50);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblRomCode, lblRomCodeValue,
                lblModule, lblModuleValue,
                lblLocation, lblLocationValue,
                lblLabel, txtLabel,
                lblMaxTemp, nudMaxTemp,
                lblOffset, nudOffset,
                chkEnabled,
                btnSave, btnCancel });
            this.FormBorderStyle   = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox       = false;
            this.MinimizeBox       = false;
            this.Name              = "SensorEditForm";
            this.StartPosition     = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text              = "Edit Sensor";

            ((System.ComponentModel.ISupportInitialize)this.nudMaxTemp).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.nudOffset).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
