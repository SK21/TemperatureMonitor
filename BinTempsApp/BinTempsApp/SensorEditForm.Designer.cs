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
        private System.Windows.Forms.Label lblBin;
        private System.Windows.Forms.NumericUpDown nudBin;
        private System.Windows.Forms.Label lblCable;
        private System.Windows.Forms.NumericUpDown nudCable;
        private System.Windows.Forms.Label lblSNum;
        private System.Windows.Forms.NumericUpDown nudSensorNum;
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
            this.lblRomCode    = new System.Windows.Forms.Label();
            this.lblRomCodeValue = new System.Windows.Forms.Label();
            this.lblModule     = new System.Windows.Forms.Label();
            this.lblModuleValue = new System.Windows.Forms.Label();
            this.lblLocation   = new System.Windows.Forms.Label();
            this.lblBin        = new System.Windows.Forms.Label();
            this.nudBin        = new System.Windows.Forms.NumericUpDown();
            this.lblCable      = new System.Windows.Forms.Label();
            this.nudCable      = new System.Windows.Forms.NumericUpDown();
            this.lblSNum       = new System.Windows.Forms.Label();
            this.nudSensorNum  = new System.Windows.Forms.NumericUpDown();
            this.lblLabel      = new System.Windows.Forms.Label();
            this.txtLabel      = new System.Windows.Forms.TextBox();
            this.lblMaxTemp    = new System.Windows.Forms.Label();
            this.nudMaxTemp    = new System.Windows.Forms.NumericUpDown();
            this.lblOffset     = new System.Windows.Forms.Label();
            this.nudOffset     = new System.Windows.Forms.NumericUpDown();
            this.chkEnabled    = new System.Windows.Forms.CheckBox();
            this.btnSave       = new System.Windows.Forms.Button();
            this.btnCancel     = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)this.nudBin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.nudCable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.nudSensorNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.nudMaxTemp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.nudOffset).BeginInit();
            this.SuspendLayout();

            // ── row positions: col1=12  col2=130  rowH=33 ────────────────────
            // row0=12  row1=45  row2=78  row3=111  row4=144  row5=177
            // row6=210  row7=243

            // ROM Code
            this.lblRomCode.AutoSize = true;
            this.lblRomCode.Location = new System.Drawing.Point(12, 15);
            this.lblRomCode.Text     = "ROM Code:";
            this.lblRomCodeValue.AutoSize = true;
            this.lblRomCodeValue.Location = new System.Drawing.Point(130, 15);
            this.lblRomCodeValue.Font     = new System.Drawing.Font("Consolas", 9F);

            // Module
            this.lblModule.AutoSize = true;
            this.lblModule.Location = new System.Drawing.Point(12, 48);
            this.lblModule.Text     = "Module:";
            this.lblModuleValue.AutoSize = true;
            this.lblModuleValue.Location = new System.Drawing.Point(130, 48);

            // Location — editable Bin / Cable / Sensor NUDs
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(12, 81);
            this.lblLocation.Text     = "Location:";

            this.lblBin.AutoSize = true;
            this.lblBin.Location = new System.Drawing.Point(130, 81);
            this.lblBin.Text     = "Bin:";

            this.nudBin.Location  = new System.Drawing.Point(158, 78);
            this.nudBin.Size      = new System.Drawing.Size(55, 23);
            this.nudBin.Minimum   = 1;
            this.nudBin.Maximum   = 255;

            this.lblCable.AutoSize = true;
            this.lblCable.Location = new System.Drawing.Point(221, 81);
            this.lblCable.Text     = "Cable:";

            this.nudCable.Location  = new System.Drawing.Point(261, 78);
            this.nudCable.Size      = new System.Drawing.Size(55, 23);
            this.nudCable.Minimum   = 1;
            this.nudCable.Maximum   = 16;

            this.lblSNum.AutoSize = true;
            this.lblSNum.Location = new System.Drawing.Point(324, 81);
            this.lblSNum.Text     = "#:";

            this.nudSensorNum.Location  = new System.Drawing.Point(340, 78);
            this.nudSensorNum.Size      = new System.Drawing.Size(55, 23);
            this.nudSensorNum.Minimum   = 1;
            this.nudSensorNum.Maximum   = 16;

            // Label
            this.lblLabel.AutoSize = true;
            this.lblLabel.Location = new System.Drawing.Point(12, 114);
            this.lblLabel.Text     = "Label:";
            this.txtLabel.Location  = new System.Drawing.Point(130, 111);
            this.txtLabel.MaxLength = 30;
            this.txtLabel.Size      = new System.Drawing.Size(200, 23);

            // Max Temp
            this.lblMaxTemp.AutoSize = true;
            this.lblMaxTemp.Location = new System.Drawing.Point(12, 147);
            this.lblMaxTemp.Text     = "Max Temp (°C):";
            this.nudMaxTemp.Location      = new System.Drawing.Point(130, 144);
            this.nudMaxTemp.Minimum       = -50;
            this.nudMaxTemp.Maximum       = 100;
            this.nudMaxTemp.DecimalPlaces = 1;
            this.nudMaxTemp.Increment     = 0.5m;
            this.nudMaxTemp.Size          = new System.Drawing.Size(80, 23);

            // Offset
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(12, 180);
            this.lblOffset.Text     = "Offset (°C):";
            this.nudOffset.Location      = new System.Drawing.Point(130, 177);
            this.nudOffset.Minimum       = -10;
            this.nudOffset.Maximum       = 10;
            this.nudOffset.DecimalPlaces = 2;
            this.nudOffset.Increment     = 0.1m;
            this.nudOffset.Size          = new System.Drawing.Size(80, 23);

            // Enabled
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(130, 213);
            this.chkEnabled.Text     = "Enabled";

            // Buttons
            this.btnSave.Location = new System.Drawing.Point(130, 243);
            this.btnSave.Size     = new System.Drawing.Size(90, 28);
            this.btnSave.Text     = "Save";
            this.btnSave.Click   += new System.EventHandler(this.btnSave_Click);

            this.btnCancel.Location     = new System.Drawing.Point(228, 243);
            this.btnCancel.Size         = new System.Drawing.Size(90, 28);
            this.btnCancel.Text         = "Cancel";
            this.btnCancel.DialogResult  = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Click        += new System.EventHandler(this.btnCancel_Click);

            // SensorEditForm
            this.AcceptButton      = this.btnSave;
            this.CancelButton      = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode     = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize        = new System.Drawing.Size(420, 290);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblRomCode, lblRomCodeValue,
                lblModule, lblModuleValue,
                lblLocation, lblBin, nudBin, lblCable, nudCable, lblSNum, nudSensorNum,
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

            ((System.ComponentModel.ISupportInitialize)this.nudBin).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.nudCable).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.nudSensorNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.nudMaxTemp).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.nudOffset).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
