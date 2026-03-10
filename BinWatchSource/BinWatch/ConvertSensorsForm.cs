using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BinWatch.Data;
using BinWatch.Models;
using BinWatch.Services;

namespace BinWatch
{
    public partial class ConvertSensorsForm : Form
    {
        private readonly SensorService _sensorService;
        private DataTable _table;
        private bool _loading;

        public ConvertSensorsForm(SensorService sensorService)
        {
            InitializeComponent();
            _sensorService = sensorService;

            // Build DataTable (cannot be in Designer.cs — CodeDOM parser restriction)
            _table = new System.Data.DataTable();
            _table.Columns.Add("_romCode",  typeof(string));
            _table.Columns.Add("RomCode",   typeof(string));
            _table.Columns.Add("Module",    typeof(string));
            _table.Columns.Add("Raw",       typeof(string));
            _table.Columns.Add("CurBin",    typeof(string));
            _table.Columns.Add("CurCable",  typeof(string));
            _table.Columns.Add("CurSensor", typeof(string));
            _table.Columns.Add("Locked",    typeof(string));
            _table.Columns.Add("NewBin",    typeof(string));
            _table.Columns.Add("NewCable",  typeof(string));
            _table.Columns.Add("NewSensor", typeof(string));
            _table.Columns.Add("_rawValue", typeof(int));
            dgv.DataSource = _table;

            // Add grid columns (object initializers also not supported in CodeDOM)
            var center = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };
            var centerBlue = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, ForeColor = System.Drawing.Color.DarkBlue };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "RomCode",   DataPropertyName = "RomCode",   HeaderText = "ROM Code",   AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Module",    DataPropertyName = "Module",    HeaderText = "Module",     Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Raw",       DataPropertyName = "Raw",       HeaderText = "Raw",        Width = 55 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "CurBin",    DataPropertyName = "CurBin",    HeaderText = "Cur Bin",    Width = 65,  DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "CurCable",  DataPropertyName = "CurCable",  HeaderText = "Cur Cable",  Width = 75,  DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "CurSensor", DataPropertyName = "CurSensor", HeaderText = "Cur Sensor", Width = 80,  DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Locked",    DataPropertyName = "Locked",    HeaderText = "Locked",     Width = 60,  DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "NewBin",    DataPropertyName = "NewBin",    HeaderText = "New Bin",    Width = 65,  DefaultCellStyle = centerBlue });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "NewCable",  DataPropertyName = "NewCable",  HeaderText = "New Cable",  Width = 75,  DefaultCellStyle = centerBlue });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "NewSensor", DataPropertyName = "NewSensor", HeaderText = "New Sensor", Width = 80,  DefaultCellStyle = centerBlue });

            foreach (var fmt in UserDataFormat.BuiltIn)
                cboPreset.Items.Add(fmt);

            LoadSensors();
            cboPreset.SelectedIndex = 0;     // triggers preset load → UpdateNewColumns
        }

        // ─── Data ────────────────────────────────────────────────────────────────

        private void LoadSensors()
        {
            _table.Rows.Clear();

            foreach (var s in _sensorService.GetAll())
            {
                // If raw was never stored, reconstruct it from the stored Bin/Cable/Sensor
                // (valid only if the sensor was always decoded with BinWatch format).
                ushort raw = s.RawUserData.HasValue
                    ? (ushort)s.RawUserData.Value
                    : (ushort)((s.BinId << 8) | (s.CableId << 4) | s.SensorNum);

                var row = _table.NewRow();
                row["_romCode"]   = s.RomCode;
                row["RomCode"]    = FormatRomCode(s.RomCode);
                row["Module"]     = s.Module?.Name ?? s.ModuleMac ?? "";
                row["Raw"]        = $"{raw:X4}";
                row["CurBin"]     = (s.BinId    + 1).ToString();
                row["CurCable"]   = (s.CableId  + 1).ToString();
                row["CurSensor"]  = (s.SensorNum + 1).ToString();
                row["Locked"]     = s.ManualLocation ? "Yes" : "";
                row["_rawValue"]  = (int)raw;
                _table.Rows.Add(row);
            }

            UpdateNewColumns();
        }

        private void UpdateNewColumns()
        {
            var fmt = CurrentFormat();
            foreach (DataRow row in _table.Rows)
            {
                ushort raw = (ushort)(int)row["_rawValue"];
                row["NewBin"]    = (fmt.DecodeBin(raw)    + 1).ToString();
                row["NewCable"]  = (fmt.DecodeCable(raw)  + 1).ToString();
                row["NewSensor"] = (fmt.DecodeSensor(raw) + 1).ToString();
            }
        }

        private UserDataFormat CurrentFormat() => new UserDataFormat
        {
            Name        = "Custom",
            BinShift    = (int)nudBinShift.Value,    BinBits    = (int)nudBinBits.Value,
            CableShift  = (int)nudCableShift.Value,  CableBits  = (int)nudCableBits.Value,
            SensorShift = (int)nudSensorShift.Value, SensorBits = (int)nudSensorBits.Value
        };

        // ─── Events ──────────────────────────────────────────────────────────────

        private void cboPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboPreset.SelectedItem is UserDataFormat fmt)) return;

            _loading = true;
            nudBinShift.Value    = fmt.BinShift;    nudBinBits.Value    = fmt.BinBits;
            nudCableShift.Value  = fmt.CableShift;  nudCableBits.Value  = fmt.CableBits;
            nudSensorShift.Value = fmt.SensorShift; nudSensorBits.Value = fmt.SensorBits;
            _loading = false;

            UpdateNewColumns();
        }

        private void nudAny_ValueChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            UpdateNewColumns();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            var fmt = CurrentFormat();
            int count = 0;

            using (var db = new AppDbContext())
            {
                foreach (DataRow row in _table.Rows)
                {
                    string romCode = row["_romCode"].ToString();
                    ushort raw     = (ushort)(int)row["_rawValue"];

                    var sensor = db.Sensors.Find(romCode);
                    if (sensor == null) continue;

                    sensor.BinId          = (byte)fmt.DecodeBin(raw);
                    sensor.CableId        = (byte)fmt.DecodeCable(raw);
                    sensor.SensorNum      = (byte)fmt.DecodeSensor(raw);
                    sensor.ManualLocation = true;
                    count++;
                }
                db.SaveChanges();
            }

            LoadSensors();

            MessageBox.Show(
                $"{count} sensor(s) updated in the database.\n\n" +
                "Location is now locked — incoming packets will not overwrite it.\n\n" +
                "If modules are online, click 'Reprogram Sensors' to write the new\n" +
                "location into each sensor's EEPROM so future packets decode correctly\n" +
                "and the lock is no longer needed.",
                "Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnReprogram_Click(object sender, EventArgs e)
        {
            var fmt   = CurrentFormat();
            int count = 0;
            var skipped = new List<string>();

            using (var db = new AppDbContext())
            {
                foreach (DataRow row in _table.Rows)
                {
                    string romCode = row["_romCode"].ToString();
                    ushort raw     = (ushort)(int)row["_rawValue"];

                    var sensor = db.Sensors.Include("Module").FirstOrDefault(s => s.RomCode == romCode);
                    if (sensor?.Module == null || sensor.Module.ModuleId == 0)
                    {
                        skipped.Add(FormatRomCode(romCode));
                        continue;
                    }

                    _sensorService.SendUserData(
                        sensor.Module.ModuleId,
                        HexToBytes(romCode),
                        (byte)fmt.DecodeBin(raw),
                        (byte)fmt.DecodeCable(raw),
                        (byte)fmt.DecodeSensor(raw));
                    count++;
                }
            }

            string msg = $"Reprogram command sent for {count} sensor(s).";
            if (skipped.Count > 0)
                msg += $"\n\nSkipped {skipped.Count} sensor(s) with no registered module:\n" +
                       string.Join("\n", skipped);
            msg += "\n\nOnce sensors are reprogrammed, you can clear 'Locked' in " +
                   "Sensor Edit so BinWatch resumes reading location from packets.";

            MessageBox.Show(msg, "Reprogram", MessageBoxButtons.OK,
                skipped.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private static string FormatRomCode(string rc)
        {
            if (rc == null || rc.Length != 16) return rc ?? "";
            return $"{rc.Substring(0,4)} {rc.Substring(4,4)} {rc.Substring(8,4)} {rc.Substring(12,4)}";
        }

        private static byte[] HexToBytes(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }
    }
}
