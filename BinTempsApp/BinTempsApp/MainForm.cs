using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BinTempsApp.Data;
using BinTempsApp.Models;
using BinTempsApp.Network;
using BinTempsApp.Services;

namespace BinTempsApp
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

            InitializeTables();

            AppServices.ModuleService.ModuleUpdated += OnModuleUpdated;
            AppServices.Parser.TemperatureReceived += OnTemperatureReceived;
            AppServices.UdpServer.Error += OnUdpError;

            LoadModules();
            LoadLatestTemperatures();
            lblStatus.Text = $"Listening on UDP port {UdpServer.DefaultPort}";
            tmrStatus.Start();
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
            _modulesTable.Columns.Add("Status", typeof(string));
            _modulesTable.Columns.Add("Last Seen", typeof(string));
            _modulesTable.Columns.Add("Firmware", typeof(string));
            _modulesTable.PrimaryKey = new[] { _modulesTable.Columns["Mac"] };

            _temperaturesTable = new DataTable();
            _temperaturesTable.Columns.Add("RomCode", typeof(string));
            _temperaturesTable.Columns.Add("Bin", typeof(string));
            _temperaturesTable.Columns.Add("Cable", typeof(string));
            _temperaturesTable.Columns.Add("Sensor", typeof(string));
            _temperaturesTable.Columns.Add("Label", typeof(string));
            _temperaturesTable.Columns.Add("Temperature", typeof(string));
            _temperaturesTable.Columns.Add("Timestamp", typeof(string));
            _temperaturesTable.Columns.Add("Module", typeof(string));
            _temperaturesTable.PrimaryKey = new[] { _temperaturesTable.Columns["RomCode"] };

            // Define columns manually so they exist before the tabs are shown
            dgvModules.AutoGenerateColumns = false;
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "Mac",        DataPropertyName = "Mac",        HeaderText = "Mac",        Visible = false });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name",       DataPropertyName = "Name",       HeaderText = "Name",       Width = 130 });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID",         DataPropertyName = "ID",         HeaderText = "ID",         Width = 50 });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "IP Address", DataPropertyName = "IP Address", HeaderText = "IP Address", Width = 115 });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status",     DataPropertyName = "Status",     HeaderText = "Status",     Width = 90 });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "Last Seen",  DataPropertyName = "Last Seen",  HeaderText = "Last Seen",  Width = 130, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvModules.Columns.Add(new DataGridViewTextBoxColumn { Name = "Firmware",   DataPropertyName = "Firmware",   HeaderText = "Firmware",   Width = 70 });

            dgvTemperatures.AutoGenerateColumns = false;
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "RomCode",     DataPropertyName = "RomCode",     HeaderText = "RomCode",     Visible = false });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Bin",         DataPropertyName = "Bin",         HeaderText = "Bin",         Width = 40 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cable",       DataPropertyName = "Cable",       HeaderText = "Cable",       Width = 50 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sensor",      DataPropertyName = "Sensor",      HeaderText = "Sensor",      Width = 55 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Label",       DataPropertyName = "Label",       HeaderText = "Label",       Width = 120 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Temperature", DataPropertyName = "Temperature", HeaderText = "Temperature", Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Timestamp",   DataPropertyName = "Timestamp",   HeaderText = "Timestamp",   Width = 130 });
            dgvTemperatures.Columns.Add(new DataGridViewTextBoxColumn { Name = "Module",      DataPropertyName = "Module",      HeaderText = "Module",      Width = 120, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            dgvModules.DataSource = _modulesTable;
            dgvTemperatures.DataSource = _temperaturesTable;
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
                        record.Timestamp, moduleName);
                }
            }
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
            }));
        }

        private void OnTemperatureReceived(object sender, TemperaturePacket packet)
        {
            if (!IsHandleCreated || IsDisposed) return;
            BeginInvoke(new Action(() =>
            {
                string romCode = packet.RomCodeHex;

                // Cache max temp on first sight of this sensor
                if (!_sensorMaxTemps.ContainsKey(romCode))
                {
                    using (var db = new AppDbContext())
                    {
                        var sensor = db.Sensors.Find(romCode);
                        _sensorMaxTemps[romCode] = sensor?.MaxTemp ?? 40.0f;
                        _sensorLabels[romCode] = sensor?.Label ?? "";
                    }
                }

                _moduleNames.TryGetValue(packet.ModuleId, out string moduleName);
                _sensorLabels.TryGetValue(romCode, out string label);

                UpdateTemperatureRow(romCode, packet.BinId, packet.CableId,
                    packet.SensorNum, label, packet.Temperature,
                    DateTime.Now, moduleName ?? $"Module {packet.ModuleId}");
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
            row["Status"] = module.Status ?? "";
            row["Last Seen"] = module.LastSeen?.ToString("HH:mm:ss dd/MM/yy") ?? "";
            row["Firmware"] = module.FirmwareVersion.ToString();
        }

        private void UpdateTemperatureRow(string romCode, byte binId, byte cableId,
            byte sensorNum, string label, float temperature, DateTime timestamp, string moduleName)
        {
            var row = _temperaturesTable.Rows.Find(romCode);
            if (row == null)
            {
                row = _temperaturesTable.NewRow();
                row["RomCode"] = romCode;
                _temperaturesTable.Rows.Add(row);
            }

            row["Bin"] = (binId + 1).ToString();
            row["Cable"] = (cableId + 1).ToString();
            row["Sensor"] = (sensorNum + 1).ToString();
            row["Label"] = label ?? "";
            row["Temperature"] = $"{temperature:F1} °C";
            row["Timestamp"] = timestamp.ToString("HH:mm:ss dd/MM/yy");
            row["Module"] = moduleName ?? "";
        }

        // -------------------------------------------------------------------------
        // Double-click to edit
        // -------------------------------------------------------------------------

        private void dgvModules_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string mac = _modulesTable.Rows[e.RowIndex]["Mac"].ToString();

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
            string romCode = _temperaturesTable.Rows[e.RowIndex]["RomCode"].ToString();

            var sensor = AppServices.SensorService.GetByRomCode(romCode);
            if (sensor == null) return;

            using (var form = new SensorEditForm(sensor))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    // Refresh caches so colour coding picks up the new max temp / label
                    _sensorMaxTemps[romCode] = sensor.MaxTemp;
                    _sensorLabels[romCode]   = sensor.Label ?? "";

                    // Refresh label in the grid row immediately
                    var row = _temperaturesTable.Rows.Find(romCode);
                    if (row != null)
                        row["Label"] = sensor.Label ?? "";
                }
            }
        }

        // -------------------------------------------------------------------------
        // Cell formatting (colour coding)
        // -------------------------------------------------------------------------

        private void dgvModules_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != dgvModules.Columns["Status"]?.Index) return;

            switch (e.Value?.ToString())
            {
                case "Online":       e.CellStyle.ForeColor = Color.Green;  break;
                case "Offline":      e.CellStyle.ForeColor = Color.Red;    break;
                case "Unregistered": e.CellStyle.ForeColor = Color.Orange; break;
            }
        }

        private void dgvTemperatures_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != dgvTemperatures.Columns["Temperature"]?.Index) return;

            var row = _temperaturesTable.Rows[e.RowIndex];
            string romCode = row["RomCode"].ToString();

            if (!_sensorMaxTemps.TryGetValue(romCode, out float maxTemp)) return;

            string tempStr = e.Value?.ToString().Replace("°C", "").Trim();
            if (float.TryParse(tempStr, out float temp) && temp > maxTemp)
            {
                e.CellStyle.BackColor = Color.LightCoral;
                e.CellStyle.ForeColor = Color.DarkRed;
            }
        }

        // -------------------------------------------------------------------------
        // Status timer
        // -------------------------------------------------------------------------

        private void tmrStatus_Tick(object sender, EventArgs e)
        {
            var srv = AppServices.UdpServer;
            var parser = AppServices.Parser;
            lblPackets.Text = $"Packets: {srv.RawPacketsReceived} raw  |  {srv.FilteredPacketsReceived} from modules  |  {parser.ParsedCount} parsed  |  {parser.LastStatus}";
        }

        private void OnUdpError(object sender, string message)
        {
            if (!IsHandleCreated || IsDisposed) return;
            BeginInvoke(new Action(() => lblStatus.Text = $"Error: {message}"));
        }

        // -------------------------------------------------------------------------
        // Form events
        // -------------------------------------------------------------------------

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrStatus.Stop();
            AppServices.Shutdown();
        }
    }
}
