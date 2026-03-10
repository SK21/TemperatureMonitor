namespace BinWatch
{
    partial class ConvertSensorsForm
    {
        private System.ComponentModel.IContainer components = null;

        // Top panel controls
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.ComboBox cboPreset;
        private System.Windows.Forms.Label lblBin;
        private System.Windows.Forms.NumericUpDown nudBinShift;
        private System.Windows.Forms.Label lblBinBits;
        private System.Windows.Forms.NumericUpDown nudBinBits;
        private System.Windows.Forms.Label lblCable;
        private System.Windows.Forms.NumericUpDown nudCableShift;
        private System.Windows.Forms.Label lblCableBits;
        private System.Windows.Forms.NumericUpDown nudCableBits;
        private System.Windows.Forms.Label lblSensor;
        private System.Windows.Forms.NumericUpDown nudSensorShift;
        private System.Windows.Forms.Label lblSensorBits;
        private System.Windows.Forms.NumericUpDown nudSensorBits;
        private System.Windows.Forms.Label lblShiftHint;

        // Grid
        private System.Windows.Forms.DataGridView dgv;

        // Bottom panel controls
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnReprogram;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblPreset = new System.Windows.Forms.Label();
            this.cboPreset = new System.Windows.Forms.ComboBox();
            this.lblShiftHint = new System.Windows.Forms.Label();
            this.lblBin = new System.Windows.Forms.Label();
            this.nudBinShift = new System.Windows.Forms.NumericUpDown();
            this.lblBinBits = new System.Windows.Forms.Label();
            this.nudBinBits = new System.Windows.Forms.NumericUpDown();
            this.lblCable = new System.Windows.Forms.Label();
            this.nudCableShift = new System.Windows.Forms.NumericUpDown();
            this.lblCableBits = new System.Windows.Forms.Label();
            this.nudCableBits = new System.Windows.Forms.NumericUpDown();
            this.lblSensor = new System.Windows.Forms.Label();
            this.nudSensorShift = new System.Windows.Forms.NumericUpDown();
            this.lblSensorBits = new System.Windows.Forms.Label();
            this.nudSensorBits = new System.Windows.Forms.NumericUpDown();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnReprogram = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBinShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBinBits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCableShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCableBits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSensorShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSensorBits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblPreset);
            this.pnlTop.Controls.Add(this.cboPreset);
            this.pnlTop.Controls.Add(this.lblShiftHint);
            this.pnlTop.Controls.Add(this.lblBin);
            this.pnlTop.Controls.Add(this.nudBinShift);
            this.pnlTop.Controls.Add(this.lblBinBits);
            this.pnlTop.Controls.Add(this.nudBinBits);
            this.pnlTop.Controls.Add(this.lblCable);
            this.pnlTop.Controls.Add(this.nudCableShift);
            this.pnlTop.Controls.Add(this.lblCableBits);
            this.pnlTop.Controls.Add(this.nudCableBits);
            this.pnlTop.Controls.Add(this.lblSensor);
            this.pnlTop.Controls.Add(this.nudSensorShift);
            this.pnlTop.Controls.Add(this.lblSensorBits);
            this.pnlTop.Controls.Add(this.nudSensorBits);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(860, 126);
            this.pnlTop.TabIndex = 1;
            // 
            // lblPreset
            // 
            this.lblPreset.AutoSize = true;
            this.lblPreset.Location = new System.Drawing.Point(8, 12);
            this.lblPreset.Name = "lblPreset";
            this.lblPreset.Size = new System.Drawing.Size(101, 17);
            this.lblPreset.TabIndex = 0;
            this.lblPreset.Text = "Source format:";
            // 
            // cboPreset
            // 
            this.cboPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPreset.Location = new System.Drawing.Point(109, 8);
            this.cboPreset.Name = "cboPreset";
            this.cboPreset.Size = new System.Drawing.Size(200, 24);
            this.cboPreset.TabIndex = 1;
            this.cboPreset.SelectedIndexChanged += new System.EventHandler(this.cboPreset_SelectedIndexChanged);
            // 
            // lblShiftHint
            // 
            this.lblShiftHint.ForeColor = System.Drawing.Color.Gray;
            this.lblShiftHint.Location = new System.Drawing.Point(8, 42);
            this.lblShiftHint.Name = "lblShiftHint";
            this.lblShiftHint.Size = new System.Drawing.Size(830, 34);
            this.lblShiftHint.TabIndex = 2;
            this.lblShiftHint.Text = "Shift = bit offset from LSB.   Bits = field width.   0 bits = field not present i" +
    "n this format.\r\nBinWatch default: Bin shift=8 bits=8   Cable shift=4 bits=4   Se" +
    "nsor shift=0 bits=4";
            // 
            // lblBin
            // 
            this.lblBin.AutoSize = true;
            this.lblBin.Location = new System.Drawing.Point(8, 92);
            this.lblBin.Name = "lblBin";
            this.lblBin.Size = new System.Drawing.Size(62, 17);
            this.lblBin.TabIndex = 3;
            this.lblBin.Text = "Bin shift:";
            // 
            // nudBinShift
            // 
            this.nudBinShift.Location = new System.Drawing.Point(72, 89);
            this.nudBinShift.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudBinShift.Name = "nudBinShift";
            this.nudBinShift.Size = new System.Drawing.Size(45, 23);
            this.nudBinShift.TabIndex = 4;
            this.nudBinShift.ValueChanged += new System.EventHandler(this.nudAny_ValueChanged);
            // 
            // lblBinBits
            // 
            this.lblBinBits.AutoSize = true;
            this.lblBinBits.Location = new System.Drawing.Point(119, 92);
            this.lblBinBits.Name = "lblBinBits";
            this.lblBinBits.Size = new System.Drawing.Size(34, 17);
            this.lblBinBits.TabIndex = 5;
            this.lblBinBits.Text = "bits:";
            // 
            // nudBinBits
            // 
            this.nudBinBits.Location = new System.Drawing.Point(155, 89);
            this.nudBinBits.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudBinBits.Name = "nudBinBits";
            this.nudBinBits.Size = new System.Drawing.Size(45, 23);
            this.nudBinBits.TabIndex = 6;
            this.nudBinBits.ValueChanged += new System.EventHandler(this.nudAny_ValueChanged);
            // 
            // lblCable
            // 
            this.lblCable.AutoSize = true;
            this.lblCable.Location = new System.Drawing.Point(202, 92);
            this.lblCable.Name = "lblCable";
            this.lblCable.Size = new System.Drawing.Size(78, 17);
            this.lblCable.TabIndex = 7;
            this.lblCable.Text = "Cable shift:";
            // 
            // nudCableShift
            // 
            this.nudCableShift.Location = new System.Drawing.Point(282, 89);
            this.nudCableShift.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudCableShift.Name = "nudCableShift";
            this.nudCableShift.Size = new System.Drawing.Size(45, 23);
            this.nudCableShift.TabIndex = 8;
            this.nudCableShift.ValueChanged += new System.EventHandler(this.nudAny_ValueChanged);
            // 
            // lblCableBits
            // 
            this.lblCableBits.AutoSize = true;
            this.lblCableBits.Location = new System.Drawing.Point(329, 92);
            this.lblCableBits.Name = "lblCableBits";
            this.lblCableBits.Size = new System.Drawing.Size(34, 17);
            this.lblCableBits.TabIndex = 9;
            this.lblCableBits.Text = "bits:";
            // 
            // nudCableBits
            // 
            this.nudCableBits.Location = new System.Drawing.Point(365, 89);
            this.nudCableBits.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudCableBits.Name = "nudCableBits";
            this.nudCableBits.Size = new System.Drawing.Size(45, 23);
            this.nudCableBits.TabIndex = 10;
            this.nudCableBits.ValueChanged += new System.EventHandler(this.nudAny_ValueChanged);
            // 
            // lblSensor
            // 
            this.lblSensor.AutoSize = true;
            this.lblSensor.Location = new System.Drawing.Point(412, 92);
            this.lblSensor.Name = "lblSensor";
            this.lblSensor.Size = new System.Drawing.Size(87, 17);
            this.lblSensor.TabIndex = 11;
            this.lblSensor.Text = "Sensor shift:";
            // 
            // nudSensorShift
            // 
            this.nudSensorShift.Location = new System.Drawing.Point(501, 89);
            this.nudSensorShift.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudSensorShift.Name = "nudSensorShift";
            this.nudSensorShift.Size = new System.Drawing.Size(45, 23);
            this.nudSensorShift.TabIndex = 12;
            this.nudSensorShift.ValueChanged += new System.EventHandler(this.nudAny_ValueChanged);
            // 
            // lblSensorBits
            // 
            this.lblSensorBits.AutoSize = true;
            this.lblSensorBits.Location = new System.Drawing.Point(548, 92);
            this.lblSensorBits.Name = "lblSensorBits";
            this.lblSensorBits.Size = new System.Drawing.Size(34, 17);
            this.lblSensorBits.TabIndex = 13;
            this.lblSensorBits.Text = "bits:";
            // 
            // nudSensorBits
            // 
            this.nudSensorBits.Location = new System.Drawing.Point(584, 89);
            this.nudSensorBits.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudSensorBits.Name = "nudSensorBits";
            this.nudSensorBits.Size = new System.Drawing.Size(45, 23);
            this.nudSensorBits.TabIndex = 14;
            this.nudSensorBits.ValueChanged += new System.EventHandler(this.nudAny_ValueChanged);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 126);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(860, 352);
            this.dgv.TabIndex = 0;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnApply);
            this.pnlBottom.Controls.Add(this.btnReprogram);
            this.pnlBottom.Controls.Add(this.btnClose);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 478);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(860, 42);
            this.pnlBottom.TabIndex = 2;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(8, 8);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(150, 27);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Apply to Database";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnReprogram
            // 
            this.btnReprogram.Location = new System.Drawing.Point(166, 8);
            this.btnReprogram.Name = "btnReprogram";
            this.btnReprogram.Size = new System.Drawing.Size(160, 27);
            this.btnReprogram.TabIndex = 1;
            this.btnReprogram.Text = "Reprogram Sensors";
            this.btnReprogram.Click += new System.EventHandler(this.btnReprogram_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(690, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 27);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ConvertSensorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 520);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "ConvertSensorsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Convert Sensor User Data";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBinShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBinBits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCableShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCableBits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSensorShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSensorBits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
