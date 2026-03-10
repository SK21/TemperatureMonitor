using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BinWatch.Data;
using BinWatch.Models;
using BinWatch.Services;

namespace BinWatch
{
    /// <summary>
    /// Popup form for defining how commercial sensor user data bytes map to
    /// Bin / Cable / Sensor, previewing the decoded values, and applying to
    /// one sensor or a range of sensors.
    /// Built entirely in code — no Designer file needed.
    /// </summary>
    public class SensorConversionForm : Form
    {
        private readonly SensorService _sensorService;
        private readonly string _romCode;
        private Sensor _sensor;
        private ushort _raw;

        // ── 4 field-definition rows ────────────────────────────────────────────
        private const int RowCount = 4;
        private readonly ComboBox[]      _cboDesc    = new ComboBox[RowCount];
        private readonly NumericUpDown[] _nudByte    = new NumericUpDown[RowCount];
        private readonly NumericUpDown[] _nudLow     = new NumericUpDown[RowCount];
        private readonly NumericUpDown[] _nudHigh    = new NumericUpDown[RowCount];
        private readonly Label[]         _lblDecoded = new Label[RowCount];

        private ComboBox _cboRange;
        private Button   _btnApply;
        private Button   _btnReprogram;
        private Button   _btnClose;

        // Default field definitions — BinWatch native format
        // (Byte 2 = high byte = UserData1, Byte 1 = low byte = UserData0)
        private static readonly (string desc, int byteNum, int low, int high)[] Defaults =
        {
            ("Bin",    2, 0, 7),
            ("Cable",  1, 4, 7),
            ("Sensor", 1, 0, 3),
            ("(none)", 1, 0, 0),
        };

        // ── Construction ───────────────────────────────────────────────────────

        public SensorConversionForm(string romCode, SensorService sensorService)
        {
            _romCode       = romCode;
            _sensorService = sensorService;
            LoadSensorData();
            BuildUI();
            UpdateDecodedValues();
        }

        private void LoadSensorData()
        {
            using (var db = new AppDbContext())
                _sensor = db.Sensors.Include("Module").FirstOrDefault(s => s.RomCode == _romCode);

            if (_sensor == null) return;

            _raw = _sensor.RawUserData.HasValue
                ? (ushort)_sensor.RawUserData.Value
                : (ushort)((_sensor.BinId << 8) | (_sensor.CableId << 4) | _sensor.SensorNum);
        }

        // ── UI construction ────────────────────────────────────────────────────

        private void BuildUI()
        {
            Font = new Font("Microsoft Sans Serif", 10F);
            Text            = "Convert Sensor User Data";
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;

            // ── Sensor info ───────────────────────────────────────────────────
            byte byte1 = (byte)(_raw & 0xFF);
            byte byte2 = (byte)((_raw >> 8) & 0xFF);

            string moduleName = _sensor?.Module?.Name ?? _sensor?.ModuleMac ?? "—";

            Controls.Add(new Label
            {
                AutoSize = true,
                Location = new Point(10, 10),
                Text     = $"ROM Code:  {FormatRomCode(_romCode)}     Module: {moduleName}"
            });
            Controls.Add(new Label
            {
                AutoSize  = true,
                Location  = new Point(10, 32),
                ForeColor = Color.DimGray,
                Text      = $"Raw:   Byte 1 (low) = 0x{byte1:X2}   Byte 2 (high) = 0x{byte2:X2}"
            });

            // ── Separator ─────────────────────────────────────────────────────
            Controls.Add(new Panel
            {
                Dock      = DockStyle.None,
                Location  = new Point(0, 58),
                Size      = new Size(2000, 1),
                BackColor = SystemColors.ControlDark
            });

            // ── Column headers ────────────────────────────────────────────────
            int[] colX = { 10, 148, 212, 276, 350 };
            int[] colW = { 128,  55,  55,  55,  80 };
            var headers = new[] { "Description", "Byte", "Bit low", "Bit high", "Decoded" };

            var hdrPnl = new Panel
            {
                Location  = new Point(0, 63),
                Size      = new Size(2000, 24),
                BackColor = SystemColors.ControlLight
            };
            for (int c = 0; c < headers.Length; c++)
                hdrPnl.Controls.Add(new Label
                {
                    Text     = headers[c],
                    Location = new Point(colX[c], 4),
                    Width    = colW[c],
                    Font     = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold)
                });
            Controls.Add(hdrPnl);

            // ── Field rows ────────────────────────────────────────────────────
            var descItems = new[] { "(none)", "Bin", "Cable", "Sensor" };
            int rowY = 95;

            for (int i = 0; i < RowCount; i++)
            {
                int idx = i; // capture for lambda

                _cboDesc[i] = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location      = new Point(colX[0], rowY),
                    Size          = new Size(colW[0], 24)
                };
                foreach (var s in descItems) _cboDesc[i].Items.Add(s);
                _cboDesc[i].SelectedItem = Defaults[i].desc;
                _cboDesc[i].SelectedIndexChanged += (s, e) => UpdateDecodedValues();

                _nudByte[i] = new NumericUpDown
                {
                    Location = new Point(colX[1], rowY),
                    Size     = new Size(colW[1], 24),
                    Minimum  = 1, Maximum = 2,
                    Value    = Defaults[i].byteNum
                };
                _nudByte[i].ValueChanged += (s, e) => UpdateDecodedValues();

                _nudLow[i] = new NumericUpDown
                {
                    Location = new Point(colX[2], rowY),
                    Size     = new Size(colW[2], 24),
                    Minimum  = 0, Maximum = 7,
                    Value    = Defaults[i].low
                };
                _nudLow[i].ValueChanged += (s, e) => UpdateDecodedValues();

                _nudHigh[i] = new NumericUpDown
                {
                    Location = new Point(colX[3], rowY),
                    Size     = new Size(colW[3], 24),
                    Minimum  = 0, Maximum = 7,
                    Value    = Defaults[i].high
                };
                _nudHigh[i].ValueChanged += (s, e) => UpdateDecodedValues();

                _lblDecoded[i] = new Label
                {
                    Location  = new Point(colX[4], rowY + 4),
                    Size      = new Size(colW[4], 20),
                    Text      = "—",
                    ForeColor = Color.DarkBlue
                };

                Controls.AddRange(new Control[]
                    { _cboDesc[i], _nudByte[i], _nudLow[i], _nudHigh[i], _lblDecoded[i] });

                rowY += 32;
            }

            // ── Separator ─────────────────────────────────────────────────────
            Controls.Add(new Panel
            {
                Location  = new Point(0, rowY + 4),
                Size      = new Size(2000, 1),
                BackColor = SystemColors.ControlDark
            });

            // ── Range selector ────────────────────────────────────────────────
            int rangeY = rowY + 14;
            Controls.Add(new Label
            {
                AutoSize = true,
                Location = new Point(10, rangeY + 4),
                Text     = "Apply to:"
            });

            _cboRange = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location      = new Point(88, rangeY),
                Size          = new Size(300, 24)
            };
            _cboRange.Items.Add("This sensor only");
            if (_sensor?.ModuleMac != null)
                _cboRange.Items.Add($"All sensors in this module  ({_sensor.Module?.Name ?? _sensor.ModuleMac})");
            _cboRange.Items.Add("All sensors");
            _cboRange.SelectedIndex = 0;
            Controls.Add(_cboRange);

            // ── Buttons ───────────────────────────────────────────────────────
            int btnY = rangeY + 38;

            _btnApply = new Button
            {
                Location = new Point(10, btnY),
                Size     = new Size(155, 27),
                Text     = "Apply to Database"
            };
            _btnApply.Click += btnApply_Click;

            _btnReprogram = new Button
            {
                Location = new Point(173, btnY),
                Size     = new Size(165, 27),
                Text     = "Reprogram Sensors"
            };
            _btnReprogram.Click += btnReprogram_Click;

            _btnClose = new Button
            {
                Location = new Point(395, btnY),
                Size     = new Size(90, 27),
                Text     = "Close"
            };
            _btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { _btnApply, _btnReprogram, _btnClose });

            ClientSize = new Size(495, btnY + 42);
        }

        // ── Live decode ────────────────────────────────────────────────────────

        private void UpdateDecodedValues()
        {
            for (int i = 0; i < RowCount; i++)
            {
                string desc = _cboDesc[i].SelectedItem?.ToString();
                if (string.IsNullOrEmpty(desc) || desc == "(none)")
                {
                    _lblDecoded[i].Text = "—";
                    continue;
                }

                int low  = (int)_nudLow[i].Value;
                int high = (int)_nudHigh[i].Value;
                if (high < low) { _lblDecoded[i].Text = "!"; continue; }

                int val = GetFieldValue(_raw, (int)_nudByte[i].Value, low, high);
                _lblDecoded[i].Text = (val + 1).ToString(); // 1-based display
            }
        }

        // ── Apply / Reprogram ──────────────────────────────────────────────────

        private void btnApply_Click(object sender, EventArgs e)
        {
            int count = 0;
            using (var db = new AppDbContext())
            {
                foreach (var s in GetTargetSensors(db))
                {
                    ushort raw = RawFor(s);
                    var (bin, cable, sensor) = Decode(raw);
                    s.BinId          = bin;
                    s.CableId        = cable;
                    s.SensorNum      = sensor;
                    s.ManualLocation = true;
                    count++;
                }
                db.SaveChanges();
            }

            MessageBox.Show(
                $"{count} sensor(s) updated.\n\n" +
                "Location is now locked — incoming packets will not overwrite it.\n\n" +
                "Click 'Reprogram Sensors' to write the new location into each sensor's\n" +
                "EEPROM so future packets decode correctly, then you can clear the lock.",
                "Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnReprogram_Click(object sender, EventArgs e)
        {
            int count = 0;
            var skipped = new List<string>();

            using (var db = new AppDbContext())
            {
                foreach (var s in GetTargetSensors(db))
                {
                    if (s.Module == null || s.Module.ModuleId == 0)
                    {
                        skipped.Add(FormatRomCode(s.RomCode));
                        continue;
                    }

                    ushort raw = RawFor(s);
                    var (bin, cable, sensor) = Decode(raw);
                    _sensorService.SendUserData(
                        s.Module.ModuleId, HexToBytes(s.RomCode), bin, cable, sensor);
                    count++;
                }
            }

            string msg = $"Reprogram command sent for {count} sensor(s).";
            if (skipped.Count > 0)
                msg += $"\n\nSkipped {skipped.Count} sensor(s) with no registered module:\n" +
                       string.Join("\n", skipped);
            MessageBox.Show(msg, "Reprogram", MessageBoxButtons.OK,
                skipped.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private List<Sensor> GetTargetSensors(AppDbContext db)
        {
            string sel = _cboRange.SelectedItem?.ToString() ?? "";
            IQueryable<Sensor> q = db.Sensors.Include("Module");

            if (sel.StartsWith("All sensors in this module"))
                q = q.Where(s => s.ModuleMac == _sensor.ModuleMac);
            else if (sel != "All sensors")
                q = q.Where(s => s.RomCode == _romCode);

            return q.ToList();
        }

        /// <summary>Decode the 4 field rows against the given raw value.</summary>
        private (byte bin, byte cable, byte sensor) Decode(ushort raw)
        {
            byte bin = 0, cable = 0, sensor = 0;
            for (int i = 0; i < RowCount; i++)
            {
                string desc = _cboDesc[i].SelectedItem?.ToString();
                if (string.IsNullOrEmpty(desc) || desc == "(none)") continue;

                int low  = (int)_nudLow[i].Value;
                int high = (int)_nudHigh[i].Value;
                if (high < low) continue;

                byte val = (byte)GetFieldValue(raw, (int)_nudByte[i].Value, low, high);
                if (desc == "Bin")    bin    = val;
                if (desc == "Cable")  cable  = val;
                if (desc == "Sensor") sensor = val;
            }
            return (bin, cable, sensor);
        }

        private static int GetFieldValue(ushort raw, int byteNum, int low, int high)
        {
            int byteVal = (byteNum == 1) ? (raw & 0xFF) : ((raw >> 8) & 0xFF);
            int width   = high - low + 1;
            int mask    = (1 << width) - 1;
            return (byteVal >> low) & mask;
        }

        private static ushort RawFor(Sensor s) =>
            s.RawUserData.HasValue
                ? (ushort)s.RawUserData.Value
                : (ushort)((s.BinId << 8) | (s.CableId << 4) | s.SensorNum);

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
