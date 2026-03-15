using BinWatch.Data;
using BinWatch.Models;
using BinWatch.Network;
using BinWatch.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BinWatch
{
    public partial class MainForm : Form
    {
        private DataTable _modulesTable;
        private DataTable _temperaturesTable;

        // Caches for live updates
        private readonly Dictionary<byte, string> _moduleNames = new Dictionary<byte, string>();
        private readonly Dictionary<string, float> _sensorMaxTemps = new Dictionary<string, float>();
        private readonly Dictionary<string, string> _sensorLabels = new Dictionary<string, string>();

        public MainForm()
        {
            InitializeComponent();

            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;

            // Configure chart (local variables not supported by VS Designer parser)
            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            chartArea.Name = "ChartArea";
            chartArea.AxisX.LabelStyle.Format = "dd/MM HH:mm";
            chartArea.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea.AxisY.Title = "°C";
            chart.ChartAreas.Add(chartArea);

            var series = new System.Windows.Forms.DataVisualization.Charting.Series();
            series.ChartArea = "ChartArea";
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series.Name = "Temperature";
            series.Color = System.Drawing.Color.SteelBlue;
            series.BorderWidth = 2;
            chart.Series.Add(series);

            InitializeTables();

            if (!AppConfig.PassiveMode)
            {
                AppServices.ModuleService.ModuleUpdated += OnModuleUpdated;
                AppServices.ModuleService.ModuleLastSeenUpdated += OnModuleLastSeenUpdated;
                AppServices.TemperatureService.BatchRecorded += OnBatchReceived;
                AppServices.UdpServer.Error += OnUdpError;
            }
            else
            {
                // Passive mode: hide active-only controls
                btnRequestDescription.Visible = false;
                btnRequestTemps.Visible = false;
                btnDiscover.Visible = false;
            }

            LoadModules();
            LoadLatestTemperatures();
            RestoreSortOrders();
            lblStatus.Text = AppConfig.PassiveMode
                ? "Passive mode (read-only)"
                : $"Listening on UDP port {UdpServer.DefaultPort}";
            tmrStatus.Start();

            dtpHistoryFrom.Value = DateTime.Today.AddDays(-7);
            dtpHistoryTo.Value = DateTime.Today.AddDays(1);

            RestoreFormBounds();

            this.Text = "BinWatch - " + Properties.Settings.Default.AppVersion;
        }

        // -------------------------------------------------------------------------
        // Setup
        // -------------------------------------------------------------------------

        private void InitializeTables()
        {
            _modulesTable = new DataTable();
            _modulesTable.Columns.Add("Mac", typeof(string));
            _modulesTable.Columns.Add("Name", typeof(string));
            _modulesTable.Columns.Add("ID", typeof(string));
            _modulesTable.Columns.Add("IP Address", typeof(string));
            _modulesTable.Columns.Add("Last Seen", typeof(string));
            _modulesTable.Columns.Add("Firmware", typeof(string));
            _modulesTable.PrimaryKey = new[] { _modulesTable.Columns["Mac"] };

            _temperaturesTable = new DataTable();
            _temperaturesTable.Columns.Add("RomCode", typeof(string));
            _temperaturesTable.Columns.Add("Bin", typeof(string));
            _temperaturesTable.Columns.Add("Cable", typeof(string));
            _temperaturesTable.Columns.Add("Sensor", typeof(string));
            _temperaturesTable.Columns.Add("Label", typeof(string));
            _temperaturesTable.Columns.Add("Temperature", typeof(float));
            _temperaturesTable.Columns.Add("Timestamp", typeof(string));
            _temperaturesTable.Columns.Add("Module", typeof(string));
            _temperaturesTable.Columns.Add("Locked", typeof(string));
            _temperaturesTable.PrimaryKey = new[] { _temperaturesTable.Columns["RomCode"] };

            // Define columns manually so they exist before the tabs are shown
            dgvModules.AutoGenerateColumns = false;
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "Mac", DataPropertyName = "Mac", HeaderText = "MAC", Width = 135 });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", DataPropertyName = "Name", HeaderText = "Name", Width = 130 });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", DataPropertyName = "ID", HeaderText = "ID", Width = 50 });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "IP Address", DataPropertyName = "IP Address", HeaderText = "IP Address", Width = 115 });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "Last Seen", DataPropertyName = "Last Seen", HeaderText = "Last Seen", Width = 130, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "Firmware", DataPropertyName = "Firmware", HeaderText = "Firmware", Width = 70 });

            dgvTemperatures.AutoGenerateColumns = false;
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "RomCode", DataPropertyName = "RomCode", HeaderText = "RomCode", Width = 180 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Bin", DataPropertyName = "Bin", HeaderText = "Bin", Width = 40 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cable", DataPropertyName = "Cable", HeaderText = "Cable", Width = 50 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sensor", DataPropertyName = "Sensor", HeaderText = "Sensor", Width = 55 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Label", DataPropertyName = "Label", HeaderText = "Label", Width = 120 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Temperature",
                DataPropertyName = "Temperature",
                HeaderText = "Temperature",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Timestamp", DataPropertyName = "Timestamp", HeaderText = "Timestamp", Width = 140 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Module", DataPropertyName = "Module", HeaderText = "Module", Width = 120, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Locked", DataPropertyName = "Locked", HeaderText = "Locked", Width = 55, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });

            dgvModules.DataSource = _modulesTable;
            dgvTemperatures.DataSource = _temperaturesTable;

            dgvModules.ColumnHeaderMouseClick += dgvModules_ColumnHeaderMouseClick;
            dgvTemperatures.ColumnHeaderMouseClick += dgvTemperatures_ColumnHeaderMouseClick;
            dgvModules.DataError += (s, e) => e.Cancel = true;
            dgvTemperatures.DataError += (s, e) => e.Cancel = true;
        }

        // -------------------------------------------------------------------------
        // Initial DB load
        // -------------------------------------------------------------------------

        private void LoadModules()
        {
            foreach (var module in AppServices.ModuleService.GetAllModules())
            {
                UpdateModuleRow(module);
                if (module.ModuleId != 0)
                    _moduleNames[module.ModuleId] = module.Name;
            }
            RefreshModuleFilter();
        }

        private void RefreshModuleFilter()
        {
            RefreshModuleCombo(cboModuleFilter, cboModuleFilter_SelectedIndexChanged);
            RefreshModuleCombo(cboHistoryModule, cboHistoryModule_SelectedIndexChanged);
            ApplyModuleFilter();
        }

        private void RefreshModuleCombo(ComboBox cbo, EventHandler handler)
        {
            string selected = cbo.SelectedItem as string;

            cbo.SelectedIndexChanged -= handler;
            cbo.Items.Clear();
            cbo.Items.Add("(All)");
            foreach (DataRow row in _modulesTable.Rows)
            {
                string name = row["Name"]?.ToString();
                if (!string.IsNullOrWhiteSpace(name) && !cbo.Items.Contains(name))
                    cbo.Items.Add(name);
            }

            int idx = selected != null ? cbo.Items.IndexOf(selected) : -1;
            cbo.SelectedIndex = idx >= 0 ? idx : 0;
            cbo.SelectedIndexChanged += handler;
        }

        private void ApplyModuleFilter()
        {
            string selected = cboModuleFilter.SelectedItem as string;
            _temperaturesTable.DefaultView.RowFilter =
                string.IsNullOrEmpty(selected) || selected == "(All)"
                    ? ""
                    : $"Module = '{selected.Replace("'", "''")}'";
        }

        private void cboModuleFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyModuleFilter();
        }

        private void cboHistoryModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshHistorySensorList();
        }

        private void chkFahrenheit_CheckedChanged(object sender, EventArgs e)
        {
            dgvTemperatures.Invalidate();
        }

        private void LoadLatestTemperatures()
        {
            var records = AppServices.TemperatureService.GetLatestPerSensor();
            if (!records.Any()) return;

            using (var db = new AppDbContext())
            {
                foreach (var record in records)
                {
                    var sensor = db.Sensors.Find(record.RomCode);
                    if (sensor == null) continue;

                    _sensorMaxTemps[record.RomCode] = sensor.MaxTemp;
                    _sensorLabels[record.RomCode] = sensor.Label ?? "";

                    string moduleName = _moduleNames.TryGetValue(0, out _)
                        ? "" : GetModuleNameById(db, sensor.ModuleMac);

                    UpdateTemperatureRow(record.RomCode, sensor.BinId, sensor.CableId,
                        sensor.SensorNum, sensor.Label, record.Temperature,
                        record.Timestamp, moduleName, sensor.ManualLocation);
                }
            }

            RefreshHistorySensorList();
        }

        private string GetModuleNameById(AppDbContext db, string moduleMac)
        {
            if (moduleMac == null) return "";
            var module = db.Modules.Find(moduleMac);
            return module?.Name ?? moduleMac;
        }

        // -------------------------------------------------------------------------
        // Event handlers (background thread → BeginInvoke to UI thread)
        // -------------------------------------------------------------------------

        private void OnModuleUpdated(object sender, ModuleUpdatedEventArgs e)
        {
            if (!IsHandleCreated || IsDisposed) return;
            BeginInvoke(new Action(() =>
            {
                UpdateModuleRow(e.Module);
                if (e.Module.ModuleId != 0)
                    _moduleNames[e.Module.ModuleId] = e.Module.Name ?? "";
                RefreshModuleFilter();
            }));
        }

        private void OnModuleLastSeenUpdated(object sender, ModuleUpdatedEventArgs e)
        {
            if (!IsHandleCreated || IsDisposed) return;
            BeginInvoke(new Action(() =>
            {
                var row = _modulesTable.Rows.Find(e.Module.MacAddress);
                if (row != null)
                    row["Last Seen"] = e.Module.LastSeen?.ToString("hh:mm tt dd/MMM/yy") ?? "";
            }));
        }

        private void OnBatchReceived(object sender, TemperatureBatchEventArgs e)
        {
            if (!IsHandleCreated || IsDisposed) return;
            BeginInvoke(new Action(() =>
            {
                string moduleName = e.ModuleName ?? (e.ModuleId != 0 ? $"Module {e.ModuleId}" : "unknown");

                foreach (var (sensor, record) in e.Entries)
                {
                    // Update caches from the already-loaded sensor — no extra DB query needed
                    _sensorMaxTemps[sensor.RomCode] = sensor.MaxTemp;
                    _sensorLabels[sensor.RomCode] = sensor.Label ?? "";

                    _moduleNames.TryGetValue(e.ModuleId, out string cachedName);

                    UpdateTemperatureRow(sensor.RomCode, sensor.BinId, sensor.CableId,
                        sensor.SensorNum, sensor.Label ?? "", record.Temperature,
                        record.Timestamp, cachedName ?? moduleName, sensor.ManualLocation);
                }

                SetStatusTimed($"Temperatures updated from {moduleName}  ({DateTime.Now:HH:mm:ss})", 10);
                Logger.Debug($"Temperatures received from {moduleName}: {string.Join(", ", e.Entries.Select(x => $"{x.Record.Temperature:F1}°C"))} at {DateTime.Now:HH:mm:ss}");
            }));
        }

        // -------------------------------------------------------------------------
        // Row update helpers
        // -------------------------------------------------------------------------

        private void UpdateModuleRow(Module module)
        {
            var row = _modulesTable.Rows.Find(module.MacAddress);
            if (row == null)
            {
                row = _modulesTable.NewRow();
                row["Mac"] = module.MacAddress;
                _modulesTable.Rows.Add(row);
            }

            row["Name"] = module.Name ?? "";
            row["ID"] = module.ModuleId == 0 ? "-" : module.ModuleId.ToString();
            row["IP Address"] = module.LastKnownIp ?? "";
            row["Last Seen"] = module.LastSeen?.ToString("hh:mm tt dd/MMM/yy") ?? "";
            row["Firmware"] = module.FirmwareVersion.ToString();
        }

        private void UpdateTemperatureRow(string romCode, byte binId, byte cableId,
            byte sensorNum, string label, float temperature, DateTime timestamp, string moduleName,
            bool manualLocation = false)
        {
            var row = _temperaturesTable.Rows.Find(romCode);
            bool isNew = row == null;
            if (isNew)
            {
                row = _temperaturesTable.NewRow();
                row["RomCode"] = romCode;
                _temperaturesTable.Rows.Add(row);
            }

            row["Bin"] = (binId + 1).ToString();
            row["Cable"] = (cableId + 1).ToString();
            row["Sensor"] = (sensorNum + 1).ToString();
            row["Label"] = label ?? "";
            row["Temperature"] = temperature;
            row["Timestamp"] = timestamp.ToString("hh:mm tt dd/MMM/yy");
            row["Module"] = moduleName ?? "";
            row["Locked"] = manualLocation ? "Yes" : "";

            if (isNew) RefreshHistorySensorList();
        }

        // -------------------------------------------------------------------------
        // Grid sorting
        // -------------------------------------------------------------------------

        private void dgvModules_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string col = dgvModules.Columns[e.ColumnIndex].Name;
            bool asc = AppConfig.ModulesSortColumn == col ? !AppConfig.ModulesSortAscending : true;
            AppConfig.ModulesSortColumn = col;
            AppConfig.ModulesSortAscending = asc;
            ApplySort(dgvModules, _modulesTable.DefaultView, col, asc);
        }

        private void dgvTemperatures_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string col = dgvTemperatures.Columns[e.ColumnIndex].Name;
            bool asc = AppConfig.TempsSortColumn == col ? !AppConfig.TempsSortAscending : true;
            AppConfig.TempsSortColumn = col;
            AppConfig.TempsSortAscending = asc;
            ApplySort(dgvTemperatures, _temperaturesTable.DefaultView, col, asc);
        }

        private void ApplySort(DataGridView dgv, System.Data.DataView view, string columnName, bool ascending)
        {
            foreach (DataGridViewColumn c in dgv.Columns)
                c.HeaderCell.SortGlyphDirection = SortOrder.None;

            if (string.IsNullOrEmpty(columnName) || !dgv.Columns.Contains(columnName)) return;

            view.Sort = $"{columnName} {(ascending ? "ASC" : "DESC")}";
            dgv.Columns[columnName].HeaderCell.SortGlyphDirection =
                ascending ? SortOrder.Ascending : SortOrder.Descending;
        }

        private void RestoreSortOrders()
        {
            ApplySort(dgvModules, _modulesTable.DefaultView,
                AppConfig.ModulesSortColumn, AppConfig.ModulesSortAscending);
            ApplySort(dgvTemperatures, _temperaturesTable.DefaultView,
                AppConfig.TempsSortColumn, AppConfig.TempsSortAscending);
        }

        // -------------------------------------------------------------------------
        // Dashboard toolbar
        // -------------------------------------------------------------------------

        private Module GetSelectedModule()
        {
            if (dgvModules.SelectedRows.Count == 0) return null;
            string mac = dgvModules.SelectedRows[0].Cells["Mac"].Value?.ToString();
            using (var db = new AppDbContext())
                return db.Modules.Find(mac);
        }

        private void btnRequestDescription_Click(object sender, EventArgs e)
        {
            var module = GetSelectedModule();
            if (module == null) return;
            AppServices.UdpServer.SendCommand(
                module.ModuleId, UdpServer.CmdSendModuleDescription, module.LastKnownIp);
        }

        private void btnRequestTemps_Click(object sender, EventArgs e)
        {
            var module = GetSelectedModule();
            if (module == null) return;
            AppServices.UdpServer.SendCommand(module.ModuleId, UdpServer.CmdUpdateAndSendTemps, module.LastKnownIp);
            SetStatusTimed($"Updating temperatures from {module.Name}…", 120);
        }

        private void btnDiscover_Click(object sender, EventArgs e)
        {
            AppServices.UdpServer.SendCommand(0, UdpServer.CmdSendModuleDescription);
            SetStatusTimed("Discovering modules…", 10);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var form = new SettingsForm())
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;

                var result = MessageBox.Show(
                    "Settings saved. Restart now to apply changes?",
                    "BinWatch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    System.Windows.Forms.Application.Restart();
                else
                    SetStatusTimed("Settings saved. Restart the app to apply changes.", 10);
            }
        }

        private void btnConvertSensors_Click(object sender, EventArgs e)
        {
            using (var form = new ConvertSensorsForm(AppServices.SensorService))
                form.ShowDialog(this);
            LoadLatestTemperatures();   // refresh grid in case locations changed
        }

        private void btnUserManual_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinWatch User Manual.pdf");
            if (!File.Exists(path))
            {
                MessageBox.Show("User manual not found:\n" + path, "BinWatch",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            System.Diagnostics.Process.Start(path);
        }

        // -------------------------------------------------------------------------
        // Double-click to edit
        // -------------------------------------------------------------------------

        private void dgvModules_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string mac = dgvModules.Rows[e.RowIndex].Cells["Mac"].Value?.ToString();

            using (var db = new AppDbContext())
            {
                var module = db.Modules.Find(mac);
                if (module == null) return;
                using (var form = new ModuleEditForm(module))
                    form.ShowDialog(this);
            }
        }

        private void dgvTemperatures_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string romCode = dgvTemperatures.Rows[e.RowIndex].Cells["RomCode"].Value?.ToString();

            var sensor = AppServices.SensorService.GetByRomCode(romCode);
            if (sensor == null) return;

            using (var form = new SensorEditForm(sensor))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    // Refresh caches so colour coding picks up the new max temp / label
                    _sensorMaxTemps[romCode] = sensor.MaxTemp;
                    _sensorLabels[romCode] = sensor.Label ?? "";

                    // Refresh location + label columns in the grid row immediately
                    var row = _temperaturesTable.Rows.Find(romCode);
                    if (row != null)
                    {
                        row["Bin"] = (sensor.BinId + 1).ToString();
                        row["Cable"] = (sensor.CableId + 1).ToString();
                        row["Sensor"] = (sensor.SensorNum + 1).ToString();
                        row["Label"] = sensor.Label ?? "";
                        row["Locked"] = sensor.ManualLocation ? "Yes" : "";
                    }
                    RefreshHistorySensorList();
                }
            }
        }

        // -------------------------------------------------------------------------
        // Cell formatting (colour coding)
        // -------------------------------------------------------------------------

        private void dgvTemperatures_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == dgvTemperatures.Columns["RomCode"]?.Index
                && e.Value is string rc && rc.Length == 16)
            {
                e.Value = $"{rc.Substring(0, 4)} {rc.Substring(4, 4)} {rc.Substring(8, 4)} {rc.Substring(12, 4)}";
                e.FormattingApplied = true;
                return;
            }

            if (e.ColumnIndex != dgvTemperatures.Columns["Temperature"]?.Index) return;

            if (e.Value is float tempC)
            {
                if (chkFahrenheit.Checked)
                {
                    float tempF = tempC * 9f / 5f + 32f;
                    e.Value = $"{tempF:F1} °F";
                }
                else
                {
                    e.Value = $"{tempC:F1} °C";
                }
                e.FormattingApplied = true;

                string romCode = dgvTemperatures.Rows[e.RowIndex].Cells["RomCode"].Value?.ToString();
                if (_sensorMaxTemps.TryGetValue(romCode, out float maxTemp) && tempC > maxTemp)
                {
                    e.CellStyle.BackColor = Color.LightCoral;
                    e.CellStyle.ForeColor = Color.DarkRed;
                }
            }
        }

        // -------------------------------------------------------------------------
        // CSV Export
        // -------------------------------------------------------------------------

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV files (*.csv)|*.csv";
                dlg.FileName = $"BinTemps_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var sensors = AppServices.SensorService.GetAll()
                    .ToDictionary(s => s.RomCode);

                var records = AppServices.TemperatureService.GetAllHistory(
                    DateTime.MinValue, DateTime.MaxValue);

                var sb = new StringBuilder();
                sb.AppendLine("Timestamp,RomCode,Label,Bin,Cable,Sensor,Module,Temperature,RawUserData");

                foreach (var r in records)
                {
                    sensors.TryGetValue(r.RomCode, out var sensor);
                    string label = sensor?.Label ?? "";
                    string bin = sensor != null ? (sensor.BinId + 1).ToString() : "";
                    string cable = sensor != null ? (sensor.CableId + 1).ToString() : "";
                    string snum = sensor != null ? (sensor.SensorNum + 1).ToString() : "";
                    string module = sensor?.Module?.Name ?? sensor?.ModuleMac ?? "";
                    string rawUserData = sensor?.RawUserData.HasValue == true
                        ? $"{sensor.RawUserData.Value:X4}" : "";

                    sb.AppendLine(string.Join(",",
                        r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                        r.RomCode,
                        $"\"{label}\"",
                        bin, cable, snum,
                        $"\"{module}\"",
                        r.Temperature.ToString("F2"),
                        rawUserData));
                }

                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                SetStatusTimed($"Exported {records.Count} records to {Path.GetFileName(dlg.FileName)}", 10);
            }
        }

        // -------------------------------------------------------------------------
        // History chart
        // -------------------------------------------------------------------------

        private void dgvTemperatures_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTemperatures.SelectedRows.Count == 0) return;
            string romCode = dgvTemperatures.SelectedRows[0].Cells["RomCode"].Value?.ToString();
            SelectHistorySensor(romCode);
        }

        private void SelectHistorySensor(string romCode)
        {
            for (int i = 0; i < cboHistorySensor.Items.Count; i++)
            {
                if (((SensorItem)cboHistorySensor.Items[i]).RomCode == romCode)
                {
                    cboHistorySensor.SelectedIndex = i;
                    return;
                }
            }
        }

        private void cboHistorySensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnLoadHistory_Click(null, EventArgs.Empty);
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabHistory)
                btnLoadHistory_Click(null, EventArgs.Empty);
        }

        private void btnLoadHistory_Click(object sender, EventArgs e)
        {
            if (cboHistorySensor.SelectedItem == null) return;

            string romCode = ((SensorItem)cboHistorySensor.SelectedItem).RomCode;
            var records = AppServices.TemperatureService.GetHistory(
                romCode, dtpHistoryFrom.Value.Date, dtpHistoryTo.Value.Date.AddDays(1));

            var series = chart.Series["Temperature"];
            series.Points.Clear();

            if (records.Count == 0)
            {
                chart.Titles.Clear();
                chart.Titles.Add("No data for selected range");
                return;
            }

            foreach (var r in records)
                series.Points.AddXY(r.Timestamp.ToOADate(), r.Temperature);

            chart.ChartAreas["ChartArea"].AxisX.LabelStyle.Format = "dd/MM HH:mm";
            chart.ChartAreas["ChartArea"].RecalculateAxesScale();

            string label = ((SensorItem)cboHistorySensor.SelectedItem).Label;
            chart.Titles.Clear();
            chart.Titles.Add($"{label}  —  {records.Count} readings");
        }

        private void btnPrintHistory_Click(object sender, EventArgs e)
        {
            var pd = new PrintDocument();
            pd.DefaultPageSettings.Landscape = true;
            pd.PrintPage += (s, args) =>
            {
                chart.Printing.PrintPaint(args.Graphics, args.MarginBounds);
            };
            using (var dlg = new PrintDialog { Document = pd })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                pd.Print();
            }
        }

        // Called whenever a new sensor appears in the temperatures table
        private void RefreshHistorySensorList()
        {
            string selected = (cboHistorySensor.SelectedItem as SensorItem)?.RomCode;
            cboHistorySensor.Items.Clear();

            string moduleFilter = cboHistoryModule.SelectedItem as string;
            bool filterActive = !string.IsNullOrEmpty(moduleFilter) && moduleFilter != "(All)";

            foreach (DataRow row in _temperaturesTable.Rows)
            {
                if (filterActive && row["Module"].ToString() != moduleFilter) continue;

                string romCode = row["RomCode"].ToString();
                string label = row["Label"].ToString();
                string loc = $"Bin {row["Bin"]} / Cable {row["Cable"]} / #{row["Sensor"]}";
                string display = string.IsNullOrEmpty(label) ? loc : $"{label}  ({loc})";
                cboHistorySensor.Items.Add(new SensorItem(romCode, display));
            }

            if (selected != null)
                SelectHistorySensor(selected);
            else if (cboHistorySensor.Items.Count > 0)
            {
                cboHistorySensor.SelectedIndex = 0;
                if (chart.Series["Temperature"].Points.Count == 0)
                    btnLoadHistory_Click(null, EventArgs.Empty);
            }
        }

        // -------------------------------------------------------------------------
        // Status timer
        // -------------------------------------------------------------------------

        private static readonly TimeSpan TempRequestInterval = TimeSpan.FromMinutes(60);
        private static readonly TimeSpan PassiveRefresh = TimeSpan.FromMinutes(1);
        private DateTime _lastTempRequest = DateTime.Now;
        private DateTime _lastPassiveRefresh = DateTime.MinValue;

        private DateTime _statusClearAt = DateTime.MinValue;

        private void SetStatusTimed(string text, int seconds)
        {
            lblStatus.Text = text;
            _statusClearAt = DateTime.Now.AddSeconds(seconds);
        }

        private void tmrStatus_Tick(object sender, EventArgs e)
        {
            if (_statusClearAt != DateTime.MinValue && DateTime.Now >= _statusClearAt)
            {
                lblStatus.Text = AppConfig.PassiveMode ? "Passive mode (read-only)" : "";
                _statusClearAt = DateTime.MinValue;
            }

            if (AppConfig.PassiveMode)
            {
                lblPackets.Text = "Passive mode";
                if ((DateTime.Now - _lastPassiveRefresh) >= PassiveRefresh)
                {
                    _lastPassiveRefresh = DateTime.Now;
                    LoadModules();
                    LoadLatestTemperatures();
                }
                return;
            }

            var srv = AppServices.UdpServer;
            lblPackets.Text = $"Packets: {srv.FilteredPacketsReceived}";

            if ((DateTime.Now - _lastTempRequest) >= TempRequestInterval)
            {
                _lastTempRequest = DateTime.Now;
                AppServices.UdpServer.SendCommand(0, UdpServer.CmdUpdateAndSendTemps);
            }
        }

        private void OnUdpError(object sender, string message)
        {
            Logger.Error($"UDP: {message}");
            if (!IsHandleCreated || IsDisposed) return;
            BeginInvoke(new Action(() => SetStatusTimed($"Error: {message}", 30)));
        }

        // -------------------------------------------------------------------------
        // Form events
        // -------------------------------------------------------------------------

        private void MainForm_Resize(object sender, EventArgs e)
        {
            btnExportCsv.Left = pnlTempsToolbar.Width - btnExportCsv.Width - 4;
            btnPrintHistory.Left = pnlHistoryToolbar.Width - btnPrintHistory.Width - 4;
            btnLoadHistory.Left = btnPrintHistory.Left - btnLoadHistory.Width - 4;
        }

        private void RestoreFormBounds()
        {
            if (AppConfig.MainFormLeft == -1) return;  // no saved position yet

            // Verify at least the title bar is visible on some screen before restoring
            bool onScreen = false;
            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (screen.WorkingArea.Contains(AppConfig.MainFormLeft + 50, AppConfig.MainFormTop + 10))
                {
                    onScreen = true;
                    break;
                }
            }
            if (!onScreen) return;

            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Left = AppConfig.MainFormLeft;
            Top = AppConfig.MainFormTop;
            if (AppConfig.MainFormWidth > 0) Width = AppConfig.MainFormWidth;
            if (AppConfig.MainFormHeight > 0) Height = AppConfig.MainFormHeight;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            btnExportCsv.Left = pnlTempsToolbar.Width - btnExportCsv.Width - 4;
            btnPrintHistory.Left = pnlHistoryToolbar.Width - btnPrintHistory.Width - 4;
            btnLoadHistory.Left = btnPrintHistory.Left - btnLoadHistory.Width - 4;
            if (!AppConfig.PassiveMode)
                AppServices.UdpServer.SendCommand(0, UdpServer.CmdSendModuleDescription | UdpServer.CmdUpdateAndSendTemps);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == System.Windows.Forms.FormWindowState.Normal)
                AppConfig.SaveFormBounds(Left, Top, Width, Height);
            else
                AppConfig.SaveFormBounds(AppConfig.MainFormLeft, AppConfig.MainFormTop,
                    AppConfig.MainFormWidth, AppConfig.MainFormHeight);

            tmrStatus.Stop();
            AppServices.Shutdown();
        }

        // -------------------------------------------------------------------------
        // Helper
        // -------------------------------------------------------------------------

        private class SensorItem
        {
            public string RomCode { get; }
            public string Label { get; }
            public SensorItem(string romCode, string label) { RomCode = romCode; Label = label; }
            public override string ToString() => Label;
        }
    }
}
