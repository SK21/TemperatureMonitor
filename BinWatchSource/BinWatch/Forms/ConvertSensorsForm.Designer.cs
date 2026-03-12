namespace BinWatch
{
    partial class ConvertSensorsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnEditFormat;
        private System.Windows.Forms.Button btnReprogram;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ToolTip toolTip;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnEditFormat = new System.Windows.Forms.Button();
            this.btnReprogram = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // dgv
            //
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.AutoGenerateColumns = false;
            this.dgv.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(860, 476);
            this.dgv.TabIndex = 0;
            this.dgv.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellDoubleClick);
            //
            // pnlBottom
            //
            this.pnlBottom.Controls.Add(this.btnEditFormat);
            this.pnlBottom.Controls.Add(this.btnReprogram);
            this.pnlBottom.Controls.Add(this.btnClose);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 476);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(860, 44);
            this.pnlBottom.TabIndex = 1;
            //
            // btnEditFormat
            //
            this.btnEditFormat.Location = new System.Drawing.Point(8, 8);
            this.btnEditFormat.Name = "btnEditFormat";
            this.btnEditFormat.Size = new System.Drawing.Size(145, 27);
            this.btnEditFormat.TabIndex = 0;
            this.btnEditFormat.Text = "Edit Format...";
            this.btnEditFormat.Click += new System.EventHandler(this.btnEditFormat_Click);
            //
            // btnReprogram
            //
            this.btnReprogram.Location = new System.Drawing.Point(161, 8);
            this.btnReprogram.Name = "btnReprogram";
            this.btnReprogram.Size = new System.Drawing.Size(160, 27);
            this.btnReprogram.TabIndex = 1;
            this.btnReprogram.Text = "Reprogram Selected";
            this.btnReprogram.Click += new System.EventHandler(this.btnReprogram_Click);
            this.toolTip.SetToolTip(this.btnReprogram, "Sends the current Bin/Cable/Sensor values to the sensor's EEPROM via UDP.\r\nThe sensor will report this location on every future temperature packet.");
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(762, 8);
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
            this.Controls.Add(this.pnlBottom);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "ConvertSensorsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text          = "Convert Sensor User Data";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
