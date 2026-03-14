namespace BinWatch
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
        private System.Windows.Forms.Button btnConvertSensors;
        private System.Windows.Forms.Button btnDiscover;
        private System.Windows.Forms.DataGridView dgvModules;

        // Temperatures tab
        private System.Windows.Forms.Panel pnlTempsToolbar;
        private System.Windows.Forms.Label lblModuleFilter;
        private System.Windows.Forms.ComboBox cboModuleFilter;
        private System.Windows.Forms.CheckBox chkFahrenheit;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.DataGridView dgvTemperatures;

        // History tab
        private System.Windows.Forms.Panel pnlHistoryToolbar;
        private System.Windows.Forms.Label lblHistoryModule;
        private System.Windows.Forms.ComboBox cboHistoryModule;
        private System.Windows.Forms.Label lblHistorySensor;
        private System.Windows.Forms.ComboBox cboHistorySensor;
        private System.Windows.Forms.Label lblHistoryFrom;
        private System.Windows.Forms.DateTimePicker dtpHistoryFrom;
        private System.Windows.Forms.Label lblHistoryTo;
        private System.Windows.Forms.DateTimePicker dtpHistoryTo;
        private System.Windows.Forms.Button btnLoadHistory;
        private System.Windows.Forms.Button btnPrintHistory;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDashboard = new System.Windows.Forms.TabPage();
            this.dgvModules = new System.Windows.Forms.DataGridView();
            this.pnlDashToolbar = new System.Windows.Forms.Panel();
            this.btnRequestDescription = new System.Windows.Forms.Button();
            this.btnRequestTemps = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnConvertSensors = new System.Windows.Forms.Button();
            this.btnDiscover = new System.Windows.Forms.Button();
            this.tabTemperatures = new System.Windows.Forms.TabPage();
            this.dgvTemperatures = new System.Windows.Forms.DataGridView();
            this.pnlTempsToolbar = new System.Windows.Forms.Panel();
            this.lblModuleFilter = new System.Windows.Forms.Label();
            this.cboModuleFilter = new System.Windows.Forms.ComboBox();
            this.chkFahrenheit = new System.Windows.Forms.CheckBox();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlHistoryToolbar = new System.Windows.Forms.Panel();
            this.lblHistoryModule = new System.Windows.Forms.Label();
            this.cboHistoryModule = new System.Windows.Forms.ComboBox();
            this.lblHistorySensor = new System.Windows.Forms.Label();
            this.cboHistorySensor = new System.Windows.Forms.ComboBox();
            this.lblHistoryFrom = new System.Windows.Forms.Label();
            this.dtpHistoryFrom = new System.Windows.Forms.DateTimePicker();
            this.lblHistoryTo = new System.Windows.Forms.Label();
            this.dtpHistoryTo = new System.Windows.Forms.DateTimePicker();
            this.btnLoadHistory = new System.Windows.Forms.Button();
            this.btnPrintHistory = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblPackets = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabDashboard
            // 
            this.tabDashboard.Controls.Add(this.dgvModules);
            this.tabDashboard.Controls.Add(this.pnlDashToolbar);
            this.tabDashboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDashboard.Location = new System.Drawing.Point(4, 25);
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Padding = new System.Windows.Forms.Padding(3);
            this.tabDashboard.Size = new System.Drawing.Size(815, 467);
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
            this.dgvModules.Location = new System.Drawing.Point(3, 47);
            this.dgvModules.Name = "dgvModules";
            this.dgvModules.ReadOnly = true;
            this.dgvModules.RowHeadersVisible = false;
            this.dgvModules.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvModules.Size = new System.Drawing.Size(809, 417);
            this.dgvModules.TabIndex = 0;
            this.dgvModules.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvModules_CellDoubleClick);
            // 
            // pnlDashToolbar
            // 
            this.pnlDashToolbar.Controls.Add(this.btnRequestDescription);
            this.pnlDashToolbar.Controls.Add(this.btnRequestTemps);
            this.pnlDashToolbar.Controls.Add(this.btnSettings);
            this.pnlDashToolbar.Controls.Add(this.btnConvertSensors);
            this.pnlDashToolbar.Controls.Add(this.btnDiscover);
            this.pnlDashToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDashToolbar.Location = new System.Drawing.Point(3, 3);
            this.pnlDashToolbar.Name = "pnlDashToolbar";
            this.pnlDashToolbar.Size = new System.Drawing.Size(809, 44);
            this.pnlDashToolbar.TabIndex = 1;
            // 
            // btnRequestDescription
            // 
            this.btnRequestDescription.Location = new System.Drawing.Point(301, 7);
            this.btnRequestDescription.Name = "btnRequestDescription";
            this.btnRequestDescription.Size = new System.Drawing.Size(136, 27);
            this.btnRequestDescription.TabIndex = 0;
            this.btnRequestDescription.Text = "Identify Module";
            this.btnRequestDescription.Click += new System.EventHandler(this.btnRequestDescription_Click);
            // 
            // btnRequestTemps
            // 
            this.btnRequestTemps.Location = new System.Drawing.Point(5, 7);
            this.btnRequestTemps.Name = "btnRequestTemps";
            this.btnRequestTemps.Size = new System.Drawing.Size(161, 27);
            this.btnRequestTemps.TabIndex = 1;
            this.btnRequestTemps.Text = "Update Temperatures";
            this.btnRequestTemps.Click += new System.EventHandler(this.btnRequestTemps_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(187, 7);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(93, 27);
            this.btnSettings.TabIndex = 2;
            this.btnSettings.Text = "Settings…";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnConvertSensors
            // 
            this.btnConvertSensors.Location = new System.Drawing.Point(458, 7);
            this.btnConvertSensors.Name = "btnConvertSensors";
            this.btnConvertSensors.Size = new System.Drawing.Size(145, 27);
            this.btnConvertSensors.TabIndex = 3;
            this.btnConvertSensors.Text = "Convert Sensors";
            this.btnConvertSensors.Click += new System.EventHandler(this.btnConvertSensors_Click);
            // 
            // btnDiscover
            // 
            this.btnDiscover.Location = new System.Drawing.Point(624, 7);
            this.btnDiscover.Name = "btnDiscover";
            this.btnDiscover.Size = new System.Drawing.Size(130, 27);
            this.btnDiscover.TabIndex = 4;
            this.btnDiscover.Text = "Discover Modules";
            this.btnDiscover.Click += new System.EventHandler(this.btnDiscover_Click);
            // 
            // tabTemperatures
            // 
            this.tabTemperatures.Controls.Add(this.dgvTemperatures);
            this.tabTemperatures.Controls.Add(this.pnlTempsToolbar);
            this.tabTemperatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabTemperatures.Location = new System.Drawing.Point(4, 25);
            this.tabTemperatures.Name = "tabTemperatures";
            this.tabTemperatures.Padding = new System.Windows.Forms.Padding(3);
            this.tabTemperatures.Size = new System.Drawing.Size(815, 467);
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
            this.dgvTemperatures.Size = new System.Drawing.Size(809, 430);
            this.dgvTemperatures.TabIndex = 0;
            this.dgvTemperatures.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTemperatures_CellDoubleClick);
            this.dgvTemperatures.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvTemperatures_CellFormatting);
            this.dgvTemperatures.SelectionChanged += new System.EventHandler(this.dgvTemperatures_SelectionChanged);
            // 
            // pnlTempsToolbar
            // 
            this.pnlTempsToolbar.Controls.Add(this.lblModuleFilter);
            this.pnlTempsToolbar.Controls.Add(this.cboModuleFilter);
            this.pnlTempsToolbar.Controls.Add(this.chkFahrenheit);
            this.pnlTempsToolbar.Controls.Add(this.btnExportCsv);
            this.pnlTempsToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTempsToolbar.Location = new System.Drawing.Point(3, 3);
            this.pnlTempsToolbar.Name = "pnlTempsToolbar";
            this.pnlTempsToolbar.Size = new System.Drawing.Size(809, 31);
            this.pnlTempsToolbar.TabIndex = 1;
            // 
            // lblModuleFilter
            // 
            this.lblModuleFilter.Location = new System.Drawing.Point(5, 6);
            this.lblModuleFilter.Name = "lblModuleFilter";
            this.lblModuleFilter.Size = new System.Drawing.Size(57, 18);
            this.lblModuleFilter.TabIndex = 1;
            this.lblModuleFilter.Text = "Module:";
            this.lblModuleFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboModuleFilter
            // 
            this.cboModuleFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboModuleFilter.Location = new System.Drawing.Point(66, 3);
            this.cboModuleFilter.Name = "cboModuleFilter";
            this.cboModuleFilter.Size = new System.Drawing.Size(180, 24);
            this.cboModuleFilter.TabIndex = 2;
            this.cboModuleFilter.SelectedIndexChanged += new System.EventHandler(this.cboModuleFilter_SelectedIndexChanged);
            // 
            // chkFahrenheit
            // 
            this.chkFahrenheit.AutoSize = true;
            this.chkFahrenheit.Location = new System.Drawing.Point(258, 6);
            this.chkFahrenheit.Name = "chkFahrenheit";
            this.chkFahrenheit.Size = new System.Drawing.Size(41, 21);
            this.chkFahrenheit.TabIndex = 3;
            this.chkFahrenheit.Text = "°F";
            this.chkFahrenheit.CheckedChanged += new System.EventHandler(this.chkFahrenheit_CheckedChanged);
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportCsv.Location = new System.Drawing.Point(692, 3);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(113, 27);
            this.btnExportCsv.TabIndex = 0;
            this.btnExportCsv.Text = "Export CSV…";
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // tabHistory
            // 
            this.tabHistory.Controls.Add(this.chart);
            this.tabHistory.Controls.Add(this.pnlHistoryToolbar);
            this.tabHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabHistory.Location = new System.Drawing.Point(4, 25);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tabHistory.Size = new System.Drawing.Size(815, 467);
            this.tabHistory.TabIndex = 2;
            this.tabHistory.Text = "History";
            // 
            // chart
            // 
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.Location = new System.Drawing.Point(3, 34);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(809, 430);
            this.chart.TabIndex = 0;
            // 
            // pnlHistoryToolbar
            // 
            this.pnlHistoryToolbar.Controls.Add(this.lblHistoryModule);
            this.pnlHistoryToolbar.Controls.Add(this.cboHistoryModule);
            this.pnlHistoryToolbar.Controls.Add(this.lblHistorySensor);
            this.pnlHistoryToolbar.Controls.Add(this.cboHistorySensor);
            this.pnlHistoryToolbar.Controls.Add(this.lblHistoryFrom);
            this.pnlHistoryToolbar.Controls.Add(this.dtpHistoryFrom);
            this.pnlHistoryToolbar.Controls.Add(this.lblHistoryTo);
            this.pnlHistoryToolbar.Controls.Add(this.dtpHistoryTo);
            this.pnlHistoryToolbar.Controls.Add(this.btnLoadHistory);
            this.pnlHistoryToolbar.Controls.Add(this.btnPrintHistory);
            this.pnlHistoryToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHistoryToolbar.Location = new System.Drawing.Point(3, 3);
            this.pnlHistoryToolbar.Name = "pnlHistoryToolbar";
            this.pnlHistoryToolbar.Size = new System.Drawing.Size(809, 31);
            this.pnlHistoryToolbar.TabIndex = 1;
            // 
            // lblHistoryModule
            // 
            this.lblHistoryModule.Location = new System.Drawing.Point(3, 6);
            this.lblHistoryModule.Name = "lblHistoryModule";
            this.lblHistoryModule.Size = new System.Drawing.Size(57, 18);
            this.lblHistoryModule.TabIndex = 0;
            this.lblHistoryModule.Text = "Module:";
            this.lblHistoryModule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboHistoryModule
            // 
            this.cboHistoryModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHistoryModule.Location = new System.Drawing.Point(63, 3);
            this.cboHistoryModule.Name = "cboHistoryModule";
            this.cboHistoryModule.Size = new System.Drawing.Size(110, 24);
            this.cboHistoryModule.TabIndex = 1;
            this.cboHistoryModule.SelectedIndexChanged += new System.EventHandler(this.cboHistoryModule_SelectedIndexChanged);
            // 
            // lblHistorySensor
            // 
            this.lblHistorySensor.Location = new System.Drawing.Point(177, 6);
            this.lblHistorySensor.Name = "lblHistorySensor";
            this.lblHistorySensor.Size = new System.Drawing.Size(57, 18);
            this.lblHistorySensor.TabIndex = 2;
            this.lblHistorySensor.Text = "Sensor:";
            this.lblHistorySensor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboHistorySensor
            // 
            this.cboHistorySensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHistorySensor.Location = new System.Drawing.Point(237, 3);
            this.cboHistorySensor.Name = "cboHistorySensor";
            this.cboHistorySensor.Size = new System.Drawing.Size(134, 24);
            this.cboHistorySensor.TabIndex = 3;
            this.cboHistorySensor.SelectedIndexChanged += new System.EventHandler(this.cboHistorySensor_SelectedIndexChanged);
            // 
            // lblHistoryFrom
            // 
            this.lblHistoryFrom.Location = new System.Drawing.Point(375, 6);
            this.lblHistoryFrom.Name = "lblHistoryFrom";
            this.lblHistoryFrom.Size = new System.Drawing.Size(50, 18);
            this.lblHistoryFrom.TabIndex = 4;
            this.lblHistoryFrom.Text = "From:";
            this.lblHistoryFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtpHistoryFrom
            // 
            this.dtpHistoryFrom.CustomFormat = "dd/MMM/yy";
            this.dtpHistoryFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpHistoryFrom.Location = new System.Drawing.Point(428, 4);
            this.dtpHistoryFrom.Name = "dtpHistoryFrom";
            this.dtpHistoryFrom.Size = new System.Drawing.Size(90, 23);
            this.dtpHistoryFrom.TabIndex = 5;
            // 
            // lblHistoryTo
            // 
            this.lblHistoryTo.Location = new System.Drawing.Point(522, 6);
            this.lblHistoryTo.Name = "lblHistoryTo";
            this.lblHistoryTo.Size = new System.Drawing.Size(32, 18);
            this.lblHistoryTo.TabIndex = 6;
            this.lblHistoryTo.Text = "To:";
            this.lblHistoryTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtpHistoryTo
            // 
            this.dtpHistoryTo.CustomFormat = "dd/MMM/yy";
            this.dtpHistoryTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpHistoryTo.Location = new System.Drawing.Point(557, 4);
            this.dtpHistoryTo.Name = "dtpHistoryTo";
            this.dtpHistoryTo.Size = new System.Drawing.Size(90, 23);
            this.dtpHistoryTo.TabIndex = 7;
            // 
            // btnLoadHistory
            // 
            this.btnLoadHistory.Location = new System.Drawing.Point(651, 2);
            this.btnLoadHistory.Name = "btnLoadHistory";
            this.btnLoadHistory.Size = new System.Drawing.Size(75, 27);
            this.btnLoadHistory.TabIndex = 8;
            this.btnLoadHistory.Text = "Load";
            this.btnLoadHistory.Click += new System.EventHandler(this.btnLoadHistory_Click);
            //
            // btnPrintHistory
            //
            this.btnPrintHistory.Location = new System.Drawing.Point(730, 2);
            this.btnPrintHistory.Name = "btnPrintHistory";
            this.btnPrintHistory.Size = new System.Drawing.Size(75, 27);
            this.btnPrintHistory.TabIndex = 9;
            this.btnPrintHistory.Text = "Print…";
            this.btnPrintHistory.Click += new System.EventHandler(this.btnPrintHistory_Click);
            //
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblPackets,
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 496);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            this.statusStrip.Size = new System.Drawing.Size(823, 24);
            this.statusStrip.TabIndex = 1;
            // 
            // lblPackets
            // 
            this.lblPackets.Name = "lblPackets";
            this.lblPackets.Size = new System.Drawing.Size(59, 19);
            this.lblPackets.Text = "Packets: 0";
            // 
            // lblStatus
            // 
            this.lblStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(74, 19);
            this.lblStatus.Text = "Initialising...";
            // 
            // tmrStatus
            // 
            this.tmrStatus.Interval = 1000;
            this.tmrStatus.Tick += new System.EventHandler(this.tmrStatus_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 520);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(602, 395);
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tabControl.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvModules)).EndInit();
            this.pnlDashToolbar.ResumeLayout(false);
            this.tabTemperatures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemperatures)).EndInit();
            this.pnlTempsToolbar.ResumeLayout(false);
            this.pnlTempsToolbar.PerformLayout();
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
