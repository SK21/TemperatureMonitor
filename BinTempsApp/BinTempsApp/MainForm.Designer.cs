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
        private System.Windows.Forms.Button btnSettings;
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
            this.components = new System.ComponentModel.Container();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDashboard = new System.Windows.Forms.TabPage();
            this.dgvModules = new System.Windows.Forms.DataGridView();
            this.pnlDashToolbar = new System.Windows.Forms.Panel();
            this.btnRequestDescription = new System.Windows.Forms.Button();
            this.btnRequestTemps = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.tabTemperatures = new System.Windows.Forms.TabPage();
            this.dgvTemperatures = new System.Windows.Forms.DataGridView();
            this.pnlTempsToolbar = new System.Windows.Forms.Panel();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlHistoryToolbar = new System.Windows.Forms.Panel();
            this.lblHistorySensor = new System.Windows.Forms.Label();
            this.cboHistorySensor = new System.Windows.Forms.ComboBox();
            this.lblHistoryFrom = new System.Windows.Forms.Label();
            this.dtpHistoryFrom = new System.Windows.Forms.DateTimePicker();
            this.lblHistoryTo = new System.Windows.Forms.Label();
            this.dtpHistoryTo = new System.Windows.Forms.DateTimePicker();
            this.btnLoadHistory = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPackets = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrStatus = new System.Windows.Forms.Timer(this.components);
            this.tabControl.SuspendLayout();
            this.tabDashboard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvModules)).BeginInit();
            this.pnlDashToolbar.SuspendLayout();
            this.tabTemperatures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemperatures)).BeginInit();
            this.pnlTempsToolbar.SuspendLayout();
            this.tabHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.pnlHistoryToolbar.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabDashboard);
            this.tabControl.Controls.Add(this.tabTemperatures);
            this.tabControl.Controls.Add(this.tabHistory);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(823, 496);
            this.tabControl.TabIndex = 0;
            // 
            // tabDashboard
            // 
            this.tabDashboard.Controls.Add(this.dgvModules);
            this.tabDashboard.Controls.Add(this.pnlDashToolbar);
            this.tabDashboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDashboard.Location = new System.Drawing.Point(4, 22);
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabDashboard.Size = new System.Drawing.Size(815, 470);
            this.tabDashboard.TabIndex = 0;
            this.tabDashboard.Text = "Dashboard";
            // 
            // dgvModules
            // 
            this.dgvModules.AllowUserToAddRows = false;
            this.dgvModules.AllowUserToDeleteRows = false;
            this.dgvModules.AllowUserToResizeRows = false;
            this.dgvModules.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvModules.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvModules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvModules.Location = new System.Drawing.Point(3, 34);
            this.dgvModules.Name = "dgvModules";
            this.dgvModules.ReadOnly = true;
            this.dgvModules.RowHeadersVisible = false;
            this.dgvModules.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvModules.Size = new System.Drawing.Size(809, 433);
            this.dgvModules.TabIndex = 0;
            this.dgvModules.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvModules_CellDoubleClick);
            this.dgvModules.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvModules_CellFormatting);
            // 
            // pnlDashToolbar
            // 
            this.pnlDashToolbar.Controls.Add(this.btnRequestDescription);
            this.pnlDashToolbar.Controls.Add(this.btnRequestTemps);
            this.pnlDashToolbar.Controls.Add(this.btnSettings);
            this.pnlDashToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDashToolbar.Location = new System.Drawing.Point(3, 3);
            this.pnlDashToolbar.Name = "pnlDashToolbar";
            this.pnlDashToolbar.Size = new System.Drawing.Size(809, 31);
            this.pnlDashToolbar.TabIndex = 1;
            // 
            // btnRequestDescription
            // 
            this.btnRequestDescription.Location = new System.Drawing.Point(3, 3);
            this.btnRequestDescription.Name = "btnRequestDescription";
            this.btnRequestDescription.Size = new System.Drawing.Size(137, 23);
            this.btnRequestDescription.TabIndex = 0;
            this.btnRequestDescription.Text = "Request Description";
            this.btnRequestDescription.Click += new System.EventHandler(this.btnRequestDescription_Click);
            // 
            // btnRequestTemps
            // 
            this.btnRequestTemps.Location = new System.Drawing.Point(147, 3);
            this.btnRequestTemps.Name = "btnRequestTemps";
            this.btnRequestTemps.Size = new System.Drawing.Size(137, 23);
            this.btnRequestTemps.TabIndex = 1;
            this.btnRequestTemps.Text = "Read Temperatures";
            this.btnRequestTemps.Click += new System.EventHandler(this.btnRequestTemps_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(291, 3);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(86, 23);
            this.btnSettings.TabIndex = 2;
            this.btnSettings.Text = "Settings…";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // tabTemperatures
            // 
            this.tabTemperatures.Controls.Add(this.dgvTemperatures);
            this.tabTemperatures.Controls.Add(this.pnlTempsToolbar);
            this.tabTemperatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabTemperatures.Location = new System.Drawing.Point(4, 22);
            this.tabTemperatures.Name = "tabTemperatures";
            this.tabTemperatures.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabTemperatures.Size = new System.Drawing.Size(815, 470);
            this.tabTemperatures.TabIndex = 1;
            this.tabTemperatures.Text = "Temperatures";
            // 
            // dgvTemperatures
            // 
            this.dgvTemperatures.AllowUserToAddRows = false;
            this.dgvTemperatures.AllowUserToDeleteRows = false;
            this.dgvTemperatures.AllowUserToResizeRows = false;
            this.dgvTemperatures.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvTemperatures.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTemperatures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTemperatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTemperatures.Location = new System.Drawing.Point(3, 34);
            this.dgvTemperatures.Name = "dgvTemperatures";
            this.dgvTemperatures.ReadOnly = true;
            this.dgvTemperatures.RowHeadersVisible = false;
            this.dgvTemperatures.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTemperatures.Size = new System.Drawing.Size(809, 433);
            this.dgvTemperatures.TabIndex = 0;
            this.dgvTemperatures.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTemperatures_CellDoubleClick);
            this.dgvTemperatures.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvTemperatures_CellFormatting);
            this.dgvTemperatures.SelectionChanged += new System.EventHandler(this.dgvTemperatures_SelectionChanged);
            // 
            // pnlTempsToolbar
            // 
            this.pnlTempsToolbar.Controls.Add(this.btnExportCsv);
            this.pnlTempsToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTempsToolbar.Location = new System.Drawing.Point(3, 3);
            this.pnlTempsToolbar.Name = "pnlTempsToolbar";
            this.pnlTempsToolbar.Size = new System.Drawing.Size(809, 31);
            this.pnlTempsToolbar.TabIndex = 1;
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportCsv.Location = new System.Drawing.Point(1323, 3);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(94, 23);
            this.btnExportCsv.TabIndex = 0;
            this.btnExportCsv.Text = "Export CSV…";
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // tabHistory
            // 
            this.tabHistory.Controls.Add(this.chart);
            this.tabHistory.Controls.Add(this.pnlHistoryToolbar);
            this.tabHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabHistory.Location = new System.Drawing.Point(4, 22);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabHistory.Size = new System.Drawing.Size(815, 470);
            this.tabHistory.TabIndex = 2;
            this.tabHistory.Text = "History";
            // 
            // chart
            // 
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.Location = new System.Drawing.Point(3, 34);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(809, 433);
            this.chart.TabIndex = 0;
            // 
            // pnlHistoryToolbar
            // 
            this.pnlHistoryToolbar.Controls.Add(this.lblHistorySensor);
            this.pnlHistoryToolbar.Controls.Add(this.cboHistorySensor);
            this.pnlHistoryToolbar.Controls.Add(this.lblHistoryFrom);
            this.pnlHistoryToolbar.Controls.Add(this.dtpHistoryFrom);
            this.pnlHistoryToolbar.Controls.Add(this.lblHistoryTo);
            this.pnlHistoryToolbar.Controls.Add(this.dtpHistoryTo);
            this.pnlHistoryToolbar.Controls.Add(this.btnLoadHistory);
            this.pnlHistoryToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHistoryToolbar.Location = new System.Drawing.Point(3, 3);
            this.pnlHistoryToolbar.Name = "pnlHistoryToolbar";
            this.pnlHistoryToolbar.Size = new System.Drawing.Size(809, 31);
            this.pnlHistoryToolbar.TabIndex = 1;
            // 
            // lblHistorySensor
            // 
            this.lblHistorySensor.Location = new System.Drawing.Point(7, 4);
            this.lblHistorySensor.Name = "lblHistorySensor";
            this.lblHistorySensor.Size = new System.Drawing.Size(43, 18);
            this.lblHistorySensor.TabIndex = 0;
            this.lblHistorySensor.Text = "Sensor:";
            this.lblHistorySensor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboHistorySensor
            // 
            this.cboHistorySensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHistorySensor.Location = new System.Drawing.Point(57, 4);
            this.cboHistorySensor.Name = "cboHistorySensor";
            this.cboHistorySensor.Size = new System.Drawing.Size(189, 21);
            this.cboHistorySensor.TabIndex = 1;
            // 
            // lblHistoryFrom
            // 
            this.lblHistoryFrom.Location = new System.Drawing.Point(259, 4);
            this.lblHistoryFrom.Name = "lblHistoryFrom";
            this.lblHistoryFrom.Size = new System.Drawing.Size(34, 18);
            this.lblHistoryFrom.TabIndex = 2;
            this.lblHistoryFrom.Text = "From:";
            this.lblHistoryFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtpHistoryFrom
            // 
            this.dtpHistoryFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHistoryFrom.Location = new System.Drawing.Point(300, 4);
            this.dtpHistoryFrom.Name = "dtpHistoryFrom";
            this.dtpHistoryFrom.Size = new System.Drawing.Size(95, 20);
            this.dtpHistoryFrom.TabIndex = 3;
            // 
            // lblHistoryTo
            // 
            this.lblHistoryTo.Location = new System.Drawing.Point(408, 4);
            this.lblHistoryTo.Name = "lblHistoryTo";
            this.lblHistoryTo.Size = new System.Drawing.Size(21, 18);
            this.lblHistoryTo.TabIndex = 4;
            this.lblHistoryTo.Text = "To:";
            this.lblHistoryTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtpHistoryTo
            // 
            this.dtpHistoryTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHistoryTo.Location = new System.Drawing.Point(435, 4);
            this.dtpHistoryTo.Name = "dtpHistoryTo";
            this.dtpHistoryTo.Size = new System.Drawing.Size(95, 20);
            this.dtpHistoryTo.TabIndex = 5;
            // 
            // btnLoadHistory
            // 
            this.btnLoadHistory.Location = new System.Drawing.Point(543, 3);
            this.btnLoadHistory.Name = "btnLoadHistory";
            this.btnLoadHistory.Size = new System.Drawing.Size(64, 23);
            this.btnLoadHistory.TabIndex = 6;
            this.btnLoadHistory.Text = "Load";
            this.btnLoadHistory.Click += new System.EventHandler(this.btnLoadHistory_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblPackets});
            this.statusStrip.Location = new System.Drawing.Point(0, 496);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            this.statusStrip.Size = new System.Drawing.Size(823, 24);
            this.statusStrip.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(70, 19);
            this.lblStatus.Text = "Initialising...";
            // 
            // lblPackets
            // 
            this.lblPackets.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblPackets.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.lblPackets.Name = "lblPackets";
            this.lblPackets.Size = new System.Drawing.Size(63, 19);
            this.lblPackets.Text = "Packets: 0";
            // 
            // tmrStatus
            // 
            this.tmrStatus.Interval = 1000;
            this.tmrStatus.Tick += new System.EventHandler(this.tmrStatus_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 520);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new System.Drawing.Size(602, 395);
            this.Name = "MainForm";
            this.Text = "BinTemps";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tabControl.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvModules)).EndInit();
            this.pnlDashToolbar.ResumeLayout(false);
            this.tabTemperatures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemperatures)).EndInit();
            this.pnlTempsToolbar.ResumeLayout(false);
            this.tabHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.pnlHistoryToolbar.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
