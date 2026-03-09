namespace BinTempsApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabDashboard;
        private System.Windows.Forms.TabPage tabTemperatures;
        private System.Windows.Forms.DataGridView dgvModules;
        private System.Windows.Forms.DataGridView dgvTemperatures;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDashboard = new System.Windows.Forms.TabPage();
            this.dgvModules = new System.Windows.Forms.DataGridView();
            this.tabTemperatures = new System.Windows.Forms.TabPage();
            this.dgvTemperatures = new System.Windows.Forms.DataGridView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();

            this.tabControl.SuspendLayout();
            this.tabDashboard.SuspendLayout();
            this.tabTemperatures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.dgvModules).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.dgvTemperatures).BeginInit();
            this.SuspendLayout();

            // tabControl
            this.tabControl.Controls.Add(this.tabDashboard);
            this.tabControl.Controls.Add(this.tabTemperatures);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;

            // tabDashboard
            this.tabDashboard.Controls.Add(this.dgvModules);
            this.tabDashboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Padding = new System.Windows.Forms.Padding(4);
            this.tabDashboard.Text = "Dashboard";

            // dgvModules
            this.dgvModules.AllowUserToAddRows = false;
            this.dgvModules.AllowUserToDeleteRows = false;
            this.dgvModules.AllowUserToResizeRows = false;
            this.dgvModules.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvModules.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvModules.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvModules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvModules.Name = "dgvModules";
            this.dgvModules.ReadOnly = true;
            this.dgvModules.RowHeadersVisible = false;
            this.dgvModules.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvModules.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvModules_CellFormatting);

            // tabTemperatures
            this.tabTemperatures.Controls.Add(this.dgvTemperatures);
            this.tabTemperatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabTemperatures.Name = "tabTemperatures";
            this.tabTemperatures.Padding = new System.Windows.Forms.Padding(4);
            this.tabTemperatures.Text = "Temperatures";

            // dgvTemperatures
            this.dgvTemperatures.AllowUserToAddRows = false;
            this.dgvTemperatures.AllowUserToDeleteRows = false;
            this.dgvTemperatures.AllowUserToResizeRows = false;
            this.dgvTemperatures.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvTemperatures.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvTemperatures.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTemperatures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTemperatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTemperatures.Name = "dgvTemperatures";
            this.dgvTemperatures.ReadOnly = true;
            this.dgvTemperatures.RowHeadersVisible = false;
            this.dgvTemperatures.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTemperatures.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvTemperatures_CellFormatting);

            // statusStrip
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.lblStatus });
            this.statusStrip.Name = "statusStrip";

            // lblStatus
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Initialising...";

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 600);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new System.Drawing.Size(700, 450);
            this.Name = "MainForm";
            this.Text = "BinTemps";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

            this.tabControl.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            this.tabTemperatures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.dgvModules).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.dgvTemperatures).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
