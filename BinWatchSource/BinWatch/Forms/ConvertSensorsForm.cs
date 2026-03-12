using System;
using System.Collections.Generic;
using System.Data;
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

        public ConvertSensorsForm(SensorService sensorService)
        {
            InitializeComponent();
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _sensorService = sensorService;

            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;

            BuildGrid();
            LoadSensors();
        }

        // ── Grid setup ────────────────────────────────────────────────────────────

        private void BuildGrid()
        {
            _table = new DataTable();
            _table.Columns.Add("_romCode", typeof(string));
            _table.Columns.Add("RomCode",  typeof(string));
            _table.Columns.Add("Module",   typeof(string));
            _table.Columns.Add("Raw",      typeof(string));
            _table.Columns.Add("Format",   typeof(string));
            _table.Columns.Add("Bin",      typeof(string));
            _table.Columns.Add("Cable",    typeof(string));
            _table.Columns.Add("Sensor",   typeof(string));
            dgv.DataSource = _table;

            var center = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RomCode", HeaderText = "ROM Code", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Module",  HeaderText = "Module",   Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Raw",     HeaderText = "Raw",      Width = 55, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Format",  HeaderText = "Format",   Width = 110 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Bin",     HeaderText = "Bin",      Width = 50, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cable",   HeaderText = "Cable",    Width = 55, DefaultCellStyle = center });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Sensor",  HeaderText = "Sensor",   Width = 60, DefaultCellStyle = center });
        }

        // ── Data ──────────────────────────────────────────────────────────────────

        private void LoadSensors()
        {
            _table.Rows.Clear();
            List<Sensor> sensors;
            using (var db = new AppDbContext())
                sensors = db.Sensors.Include("Module").Include("Format").ToList();

            foreach (var s in sensors)
            {
                ushort raw = s.RawUserData.HasValue
                    ? (ushort)s.RawUserData.Value
                    : (ushort)((s.BinId << 8) | (s.CableId << 4) | s.SensorNum);

                var row = _table.NewRow();
                row["_romCode"] = s.RomCode;
                row["RomCode"]  = FormatRomCode(s.RomCode);
                row["Module"]   = s.Module?.Name ?? s.ModuleMac ?? "";
                row["Raw"]      = $"{raw:X4}";
                row["Format"]   = s.Format?.Name ?? "";
                row["Bin"]      = (s.BinId     + 1).ToString();
                row["Cable"]    = (s.CableId   + 1).ToString();
                row["Sensor"]   = (s.SensorNum + 1).ToString();
                _table.Rows.Add(row);
            }
        }

        // ── Events ────────────────────────────────────────────────────────────────

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            OpenEditForm();
        }

        private void btnEditFormat_Click(object sender, EventArgs e) => OpenEditForm();

        private void btnReprogram_Click(object sender, EventArgs e)
        {
            var romCodes = GetSelectedRomCodes();
            if (romCodes.Count == 0)
            {
                MessageBox.Show("Select one or more sensors to reprogram.", "Reprogram",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int count = 0;
            var skipped = new List<string>();
            using (var db = new AppDbContext())
            {
                var sensors = db.Sensors.Include("Module")
                                .Where(s => romCodes.Contains(s.RomCode)).ToList();
                foreach (var sensor in sensors)
                {
                    if (sensor.Module == null || sensor.Module.ModuleId == 0)
                    {
                        skipped.Add(FormatRomCode(sensor.RomCode));
                        continue;
                    }
                    _sensorService.SendUserData(
                        sensor.Module.ModuleId, HexToBytes(sensor.RomCode),
                        sensor.BinId, sensor.CableId, sensor.SensorNum);
                    count++;
                }
            }

            string msg = $"Reprogram command sent for {count} sensor(s).";
            if (skipped.Count > 0)
                msg += $"\n\nSkipped {skipped.Count} sensor(s) with no registered module:\n" +
                       string.Join("\n", skipped);
            msg += "\n\nOnce reprogrammed, clear 'Locked' via Sensor Edit so BinWatch\n" +
                   "resumes reading location from incoming packets.";
            MessageBox.Show(msg, "Reprogram", MessageBoxButtons.OK,
                skipped.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void OpenEditForm()
        {
            var romCodes = GetSelectedRomCodes();
            if (romCodes.Count == 0) return;

            using (var form = new UserDataEditForm(romCodes, _sensorService))
            {
                if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    LoadSensors();
            }
        }

        private List<string> GetSelectedRomCodes()
        {
            var list = new List<string>();
            foreach (DataGridViewRow row in dgv.SelectedRows)
            {
                if (row.DataBoundItem is System.Data.DataRowView drv)
                    list.Add(drv.Row["_romCode"].ToString());
            }
            return list;
        }

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
