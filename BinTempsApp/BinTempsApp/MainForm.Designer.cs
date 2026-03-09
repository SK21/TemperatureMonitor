namespace BinTempsApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // Tabs
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabDashboard;
        private System.Windows.Forms.TabPage tabTemperatures;
        private System.Windows.Forms.TabPage tabHistory;

        // Dashboard tab
        private System.Windows.Forms.Panel pnlDashToolbar;
        private System.Windows.Forms.Button btnRequestDescription;
        private System.Windows.Forms.Button btnRequestTemps;
        private System.Windows.Forms.DataGridView dgvModules;

        // Temperatures tab
        private System.Windows.Forms.Panel pnlTempsToolbar;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.DataGridView dgvTemperatures;

        // History tab
        private System.Windows.Forms.Panel pnlHistoryToolbar;
        private System.Windows.Forms.Label lblHistorySensor;
        private System.Windows.Forms.ComboBox cboHistorySensor;
        private System.Windows.Forms.Label lblHistoryFrom;
        private System.Windows.Forms.DateTimePicker dtpHistoryFrom;
        private System.Windows.Forms.Label lblHistoryTo;
        private System.Windows.Forms.DateTimePicker dtpHistoryTo;
        private System.Windows.Forms.Button btnLoadHistory;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;

        // Status strip
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblPackets;
        private System.Windows.Forms.Timer tmrStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl             = new System.Windows.Forms.TabControl();
            this.tabDashboard           = new System.Windows.Forms.TabPage();
            this.pnlDashToolbar         = new System.Windows.Forms.Panel();
            this.btnRequestDescription  = new System.Windows.Forms.Button();
            this.btnRequestTemps        = new System.Windows.Forms.Button();
            this.dgvModules             = new System.Windows.Forms.DataGridView();
            this.tabTemperatures    = new System.Windows.Forms.TabPage();
            this.pnlTempsToolbar    = new System.Windows.Forms.Panel();
            this.btnExportCsv       = new System.Windows.Forms.Button();
            this.dgvTemperatures    = new System.Windows.Forms.DataGridView();
            this.tabHistory         = new System.Windows.Forms.TabPage();
            this.pnlHistoryToolbar  = new System.Windows.Forms.Panel();
            this.lblHistorySensor   = new System.Windows.Forms.Label();
            this.cboHistorySensor   = new System.Windows.Forms.ComboBox();
            this.lblHistoryFrom     = new System.Windows.Forms.Label();
            this.dtpHistoryFrom     = new System.Windows.Forms.DateTimePicker();
            this.lblHistoryTo       = new System.Windows.Forms.Label();
            this.dtpHistoryTo       = new System.Windows.Forms.DateTimePicker();
            this.btnLoadHistory     = new System.Windows.Forms.Button();
            this.chart              = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.statusStrip        = new System.Windows.Forms.StatusStrip();
            this.lblStatus          = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPackets         = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrStatus          = new System.Windows.Forms.Timer();

            this.tabControl.SuspendLayout();
            this.tabDashboard.SuspendLayout();
            this.tabTemperatures.SuspendLayout();
            this.pnlDashToolbar.SuspendLayout();
            this.pnlTempsToolbar.SuspendLayout();
            this.tabHistory.SuspendLayout();
            this.pnlHistoryToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.dgvModules).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.dgvTemperatures).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.chart).BeginInit();
            this.SuspendLayout();

            // ── tabControl ───────────────────────────────────────────────────
            this.tabControl.Controls.Add(this.tabDashboard);
            this.tabControl.Controls.Add(this.tabTemperatures);
            this.tabControl.Controls.Add(this.tabHistory);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;

            // ── tabDashboard ─────────────────────────────────────────────────
            this.tabDashboard.Controls.Add(this.dgvModules);
            this.tabDashboard.Controls.Add(this.pnlDashToolbar);
            this.tabDashboard.Dock    = System.Windows.Forms.DockStyle.Fill;
            this.tabDashboard.Name    = "tabDashboard";
            this.tabDashboard.Padding = new System.Windows.Forms.Padding(4);
            this.tabDashboard.Text    = "Dashboard";

            // pnlDashToolbar
            this.pnlDashToolbar.Controls.Add(this.btnRequestDescription);
            this.pnlDashToolbar.Controls.Add(this.btnRequestTemps);
            this.pnlDashToolbar.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlDashToolbar.Height = 36;
            this.pnlDashToolbar.Name   = "pnlDashToolbar";

            this.btnRequestDescription.Location = new System.Drawing.Point(4, 4);
            this.btnRequestDescription.Size     = new System.Drawing.Size(160, 26);
            this.btnRequestDescription.Text     = "Request Description";
            this.btnRequestDescription.Click   += new System.EventHandler(this.btnRequestDescription_Click);

            this.btnRequestTemps.Location = new System.Drawing.Point(172, 4);
            this.btnRequestTemps.Size     = new System.Drawing.Size(160, 26);
            this.btnRequestTemps.Text     = "Read Temperatures";
            this.btnRequestTemps.Click   += new System.EventHandler(this.btnRequestTemps_Click);

            // dgvModules
            this.dgvModules.AllowUserToAddRows    = false;
            this.dgvModules.AllowUserToDeleteRows = false;
            this.dgvModules.AllowUserToResizeRows = false;
            this.dgvModules.AutoSizeColumnsMode   = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvModules.BackgroundColor       = System.Drawing.SystemColors.Window;
            this.dgvModules.BorderStyle           = System.Windows.Forms.BorderStyle.None;
            this.dgvModules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvModules.Dock            = System.Windows.Forms.DockStyle.Fill;
            this.dgvModules.Name            = "dgvModules";
            this.dgvModules.ReadOnly        = true;
            this.dgvModules.RowHeadersVisible = false;
            this.dgvModules.SelectionMode   = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvModules.CellFormatting  += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvModules_CellFormatting);
            this.dgvModules.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvModules_CellDoubleClick);

            // ── tabTemperatures ──────────────────────────────────────────────
            this.tabTemperatures.Controls.Add(this.dgvTemperatures);
            this.tabTemperatures.Controls.Add(this.pnlTempsToolbar);
            this.tabTemperatures.Dock    = System.Windows.Forms.DockStyle.Fill;
            this.tabTemperatures.Name    = "tabTemperatures";
            this.tabTemperatures.Padding = new System.Windows.Forms.Padding(4);
            this.tabTemperatures.Text    = "Temperatures";

            // pnlTempsToolbar
            this.pnlTempsToolbar.Controls.Add(this.btnExportCsv);
            this.pnlTempsToolbar.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTempsToolbar.Height = 36;
            this.pnlTempsToolbar.Name   = "pnlTempsToolbar";

            // btnExportCsv
            this.btnExportCsv.Anchor   = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnExportCsv.Location = new System.Drawing.Point(800, 4);
            this.btnExportCsv.Size     = new System.Drawing.Size(110, 26);
            this.btnExportCsv.Text     = "Export CSV…";
            this.btnExportCsv.Click   += new System.EventHandler(this.btnExportCsv_Click);

            // dgvTemperatures
            this.dgvTemperatures.AllowUserToAddRows    = false;
            this.dgvTemperatures.AllowUserToDeleteRows = false;
            this.dgvTemperatures.AllowUserToResizeRows = false;
            this.dgvTemperatures.AutoSizeColumnsMode   = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvTemperatures.BackgroundColor       = System.Drawing.SystemColors.Window;
            this.dgvTemperatures.BorderStyle           = System.Windows.Forms.BorderStyle.None;
            this.dgvTemperatures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTemperatures.Dock            = System.Windows.Forms.DockStyle.Fill;
            this.dgvTemperatures.Name            = "dgvTemperatures";
            this.dgvTemperatures.ReadOnly        = true;
            this.dgvTemperatures.RowHeadersVisible = false;
            this.dgvTemperatures.SelectionMode   = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTemperatures.CellFormatting  += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvTemperatures_CellFormatting);
            this.dgvTemperatures.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTemperatures_CellDoubleClick);
            this.dgvTemperatures.SelectionChanged += new System.EventHandler(this.dgvTemperatures_SelectionChanged);

            // ── tabHistory ───────────────────────────────────────────────────
            this.tabHistory.Controls.Add(this.chart);
            this.tabHistory.Controls.Add(this.pnlHistoryToolbar);
            this.tabHistory.Dock    = System.Windows.Forms.DockStyle.Fill;
            this.tabHistory.Name    = "tabHistory";
            this.tabHistory.Padding = new System.Windows.Forms.Padding(4);
            this.tabHistory.Text    = "History";

            // pnlHistoryToolbar
            this.pnlHistoryToolbar.Controls.Add(this.lblHistorySensor);
            this.pnlHistoryToolbar.Controls.Add(this.cboHistorySensor);
            this.pnlHistoryToolbar.Controls.Add(this.lblHistoryFrom);
            this.pnlHistoryToolbar.Controls.Add(this.dtpHistoryFrom);
            this.pnlHistoryToolbar.Controls.Add(this.lblHistoryTo);
            this.pnlHistoryToolbar.Controls.Add(this.dtpHistoryTo);
            this.pnlHistoryToolbar.Controls.Add(this.btnLoadHistory);
            this.pnlHistoryToolbar.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlHistoryToolbar.Height = 36;
            this.pnlHistoryToolbar.Name   = "pnlHistoryToolbar";

            // lblHistorySensor  x=8  (label 50 + gap 8 = 58)
            this.lblHistorySensor.AutoSize  = false;
            this.lblHistorySensor.Size      = new System.Drawing.Size(50, 21);
            this.lblHistorySensor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblHistorySensor.Location  = new System.Drawing.Point(8, 5);
            this.lblHistorySensor.Text      = "Sensor:";
            // cboHistorySensor  x=66  (combo 220 + gap 16 = 236)
            this.cboHistorySensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHistorySensor.Location = new System.Drawing.Point(66, 5);
            this.cboHistorySensor.Size     = new System.Drawing.Size(220, 23);
            this.cboHistorySensor.Name     = "cboHistorySensor";
            // lblHistoryFrom  x=302  (label 40 + gap 8 = 48)
            this.lblHistoryFrom.AutoSize  = false;
            this.lblHistoryFrom.Size      = new System.Drawing.Size(40, 21);
            this.lblHistoryFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblHistoryFrom.Location  = new System.Drawing.Point(302, 5);
            this.lblHistoryFrom.Text      = "From:";
            // dtpHistoryFrom  x=350  (picker 110 + gap 16 = 126)
            this.dtpHistoryFrom.Format   = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHistoryFrom.Location = new System.Drawing.Point(350, 5);
            this.dtpHistoryFrom.Size     = new System.Drawing.Size(110, 23);
            this.dtpHistoryFrom.Name     = "dtpHistoryFrom";
            // lblHistoryTo  x=476  (label 24 + gap 8 = 32)
            this.lblHistoryTo.AutoSize  = false;
            this.lblHistoryTo.Size      = new System.Drawing.Size(24, 21);
            this.lblHistoryTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblHistoryTo.Location  = new System.Drawing.Point(476, 5);
            this.lblHistoryTo.Text      = "To:";
            // dtpHistoryTo  x=508  (picker 110 + gap 16 = 126)
            this.dtpHistoryTo.Format   = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHistoryTo.Location = new System.Drawing.Point(508, 5);
            this.dtpHistoryTo.Size     = new System.Drawing.Size(110, 23);
            this.dtpHistoryTo.Name     = "dtpHistoryTo";
            // btnLoadHistory  x=634
            this.btnLoadHistory.Location = new System.Drawing.Point(634, 4);
            this.btnLoadHistory.Size     = new System.Drawing.Size(75, 26);
            this.btnLoadHistory.Text     = "Load";
            this.btnLoadHistory.Click   += new System.EventHandler(this.btnLoadHistory_Click);

            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.Name = "chart";

            // ── statusStrip ──────────────────────────────────────────────────
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.lblStatus, this.lblPackets });
            this.statusStrip.Name = "statusStrip";

            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Initialising...";

            this.lblPackets.Name        = "lblPackets";
            this.lblPackets.Text        = "Packets: 0";
            this.lblPackets.Alignment   = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblPackets.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;

            this.tmrStatus.Interval = 1000;
            this.tmrStatus.Tick    += new System.EventHandler(this.tmrStatus_Tick);

            // ── MainForm ─────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(960, 600);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize     = new System.Drawing.Size(700, 450);
            this.Name            = "MainForm";
            this.Text            = "BinTemps";
            this.FormClosing    += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Resize         += new System.EventHandler(this.MainForm_Resize);

            this.tabControl.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            this.pnlDashToolbar.ResumeLayout(false);
            this.tabTemperatures.ResumeLayout(false);
            this.pnlTempsToolbar.ResumeLayout(false);
            this.tabHistory.ResumeLayout(false);
            this.pnlHistoryToolbar.ResumeLayout(false);
            this.pnlHistoryToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.dgvModules).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.dgvTemperatures).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.chart).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
