using BinWatch.Data;
using BinWatch.Models;
using BinWatch.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BinWatch
{
    /// <summary>
    /// Edit the user-data format for one or more sensors.
    /// Built entirely in code — no Designer file.
    /// </summary>
    public class UserDataEditForm : Form
    {
        private readonly List<string> _romCodes;
        private readonly SensorService _sensorService;
        private List<Sensor> _sensors;

        private TextBox _txtName;
        private ComboBox _cboLoad;
        private bool _loading;

        private const int Rows = 3;
        private readonly ComboBox[] _cboDesc = new ComboBox[Rows];
        private readonly ComboBox[] _cboByte = new ComboBox[Rows];
        private readonly NumericUpDown[] _nudLow = new NumericUpDown[Rows];
        private readonly NumericUpDown[] _nudHigh = new NumericUpDown[Rows];

        private DataGridView _dgvPreview;
        private DataTable _previewTable;

        public UserDataEditForm(List<string> romCodes, SensorService sensorService)
        {
            _romCodes = romCodes;
            _sensorService = sensorService;

            using (var db = new AppDbContext())
                _sensors = db.Sensors.Include("Module").Include("Format")
                             .Where(s => _romCodes.Contains(s.RomCode)).ToList();

            BuildUI();
            LoadInitialFormat();
            UpdatePreview();
        }

        // ── UI construction ───────────────────────────────────────────────────────

        private void BuildUI()
        {
            Font = new Font("Microsoft Sans Serif", 10F);
            Text = "User Data Format";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox   = false;
            MinimizeBox   = false;
            ShowInTaskbar = false;
            Icon          = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // Column x positions (absolute)
            const int xDesc = 10; const int wDesc = 118;
            const int xByte = 136; const int wByte = 50;
            const int xLow = 194; const int wNud = 52;
            const int xDash = 250;
            const int xHigh = 264;
            const int rowH = 32;
            const int formW = 580;

            int y = 12;

            // Format name + Load row
            Controls.Add(new Label { Text = "Format name:", Location = new Point(xDesc, y + 3), AutoSize = true });
            _txtName = new TextBox { Location = new Point(115, y), Size = new Size(185, 24) };
            Controls.Add(_txtName);
            Controls.Add(new Label { Text = "Load saved:", Location = new Point(315, y + 3), AutoSize = true });
            _cboLoad = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(403, y), Size = new Size(165, 24) };
            PopulateLoadCombo();
            _cboLoad.SelectedIndexChanged += cboLoad_SelectedIndexChanged;
            Controls.Add(_cboLoad);

            // Column header bar — panel sits at x=0 so labels use absolute x directly
            y = 48;
            var hdr = new Panel { Location = new Point(0, y), Size = new Size(formW, 22), BackColor = SystemColors.ControlLight };
            hdr.Controls.Add(new Label { Text = "Description", Location = new Point(xDesc, 3), Width = wDesc, Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold) });
            hdr.Controls.Add(new Label { Text = "Byte", Location = new Point(xByte, 3), Width = wByte, Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold) });
            hdr.Controls.Add(new Label { Text = "Bit low", Location = new Point(xLow, 3), Width = 65, Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold) });
            hdr.Controls.Add(new Label { Text = "Bit high", Location = new Point(xHigh, 3), Width = 65, Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold) });
            Controls.Add(hdr);

            // 4 field-definition rows
            y = 78;
            string[] descItems = { "-", "Bin", "Cable", "Sensor" };
            for (int i = 0; i < Rows; i++)
            {
                _cboDesc[i] = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(xDesc, y), Size = new Size(wDesc, 24) };
                foreach (var s in descItems) _cboDesc[i].Items.Add(s);
                _cboDesc[i].SelectedIndex = 0;
                _cboDesc[i].SelectedIndexChanged += OnFieldChanged;

                _cboByte[i] = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(xByte, y), Size = new Size(wByte, 24) };
                _cboByte[i].Items.Add("1");
                _cboByte[i].Items.Add("2");
                _cboByte[i].SelectedIndex = 0;
                _cboByte[i].SelectedIndexChanged += OnFieldChanged;

                _nudLow[i] = new NumericUpDown { Location = new Point(xLow, y), Size = new Size(wNud, 24), Minimum = 0, Maximum = 7 };
                _nudLow[i].ValueChanged += OnFieldChanged;

                Controls.Add(new Label { Text = "–", Location = new Point(xDash, y + 5), AutoSize = true });

                _nudHigh[i] = new NumericUpDown { Location = new Point(xHigh, y), Size = new Size(wNud, 24), Minimum = 0, Maximum = 7, Value = 7 };
                _nudHigh[i].ValueChanged += OnFieldChanged;

                Controls.AddRange(new Control[] { _cboDesc[i], _cboByte[i], _nudLow[i], _nudHigh[i] });
                y += rowH;
            }

            // Separator
            y += 4;
            Controls.Add(new Panel { Location = new Point(0, y), Size = new Size(formW, 1), BackColor = SystemColors.ControlDark });
            y += 10;

            // Preview label + grid
            Controls.Add(new Label { Text = "Preview:", Location = new Point(xDesc, y), AutoSize = true });
            y += 22;

            _previewTable = new DataTable();
            _previewTable.Columns.Add("RomCode", typeof(string));
            _previewTable.Columns.Add("Raw", typeof(string));
            _previewTable.Columns.Add("Bin", typeof(string));
            _previewTable.Columns.Add("Cable", typeof(string));
            _previewTable.Columns.Add("Sensor", typeof(string));

            var center = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };
            _dgvPreview = new DataGridView
            {
                Location = new Point(xDesc, y),
                Size = new Size(formW - 20, 120),
                DataSource = _previewTable,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.FixedSingle
            };
            _dgvPreview.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RomCode", HeaderText = "ROM Code", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _dgvPreview.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Raw", HeaderText = "Raw", Width = 55, DefaultCellStyle = center });
            _dgvPreview.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Bin", HeaderText = "New Bin", Width = 65, DefaultCellStyle = center });
            _dgvPreview.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cable", HeaderText = "New Cable", Width = 75, DefaultCellStyle = center });
            _dgvPreview.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Sensor", HeaderText = "New Sensor", Width = 80, DefaultCellStyle = center });
            Controls.Add(_dgvPreview);
            y += 128;

            // Buttons
            var btnSave = new Button { Text = "Save && Apply", Location = new Point(xDesc, y), Size = new Size(130, 27) };
            var btnCancel = new Button { Text = "Cancel", Location = new Point(formW - 100, y), Size = new Size(90, 27) };
            btnSave.Click += btnSave_Click;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.AddRange(new Control[] { btnSave, btnCancel });

            ClientSize = new Size(formW, y + 44);
        }

        private void PopulateLoadCombo()
        {
            _cboLoad.Items.Clear();
            _cboLoad.Items.Add("(select to load)");
            using (var db = new AppDbContext())
                foreach (var f in db.Formats.OrderBy(f => f.Name).ToList())
                    _cboLoad.Items.Add(f);
            _cboLoad.SelectedIndex = 0;
        }

        private void LoadInitialFormat()
        {
            UserDataFormatDef fmt = null;

            // Use the format already assigned to the first sensor
            if (_sensors.Count > 0 && _sensors[0].Format != null)
                fmt = _sensors[0].Format;

            // Fall back to BinWatch from DB
            if (fmt == null)
                using (var db = new AppDbContext())
                    fmt = db.Formats.FirstOrDefault(f => f.Name == "BinWatch");

            if (fmt != null)
                LoadFormatIntoControls(fmt);
        }

        private void LoadFormatIntoControls(UserDataFormatDef fmt)
        {
            _loading = true;
            _txtName.Text = fmt.Name ?? "";
            for (int i = 0; i < Rows; i++)
            {
                var (desc, byteNum, low, high) = fmt.GetRow(i);
                string d = string.IsNullOrEmpty(desc) ? "-" : desc;
                _cboDesc[i].SelectedItem = _cboDesc[i].Items.Contains(d) ? (object)d : _cboDesc[i].Items[0];
                _cboByte[i].SelectedIndex = (byteNum == 2) ? 1 : 0;
                _nudLow[i].Value = Math.Max(0, Math.Min(7, low));
                _nudHigh[i].Value = Math.Max(0, Math.Min(7, high));
            }
            _loading = false;
        }

        // ── Events ────────────────────────────────────────────────────────────────

        private void cboLoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cboLoad.SelectedItem is UserDataFormatDef fmt)
                LoadFormatIntoControls(fmt);
        }

        private void OnFieldChanged(object sender, EventArgs e)
        {
            if (!_loading) UpdatePreview();
        }

        // ── Preview ───────────────────────────────────────────────────────────────

        private void UpdatePreview()
        {
            var fmt = CurrentFormat();
            _previewTable.Rows.Clear();
            foreach (var s in _sensors)
            {
                ushort raw = s.RawUserData.HasValue
                    ? (ushort)s.RawUserData.Value
                    : (ushort)((s.BinId << 8) | (s.CableId << 4) | s.SensorNum);

                var (bin, cable, sensor) = fmt.Decode(raw);
                var row = _previewTable.NewRow();
                row["RomCode"] = FormatRomCode(s.RomCode);
                row["Raw"] = $"{raw:X4}";
                row["Bin"] = (bin + 1).ToString();
                row["Cable"] = (cable + 1).ToString();
                row["Sensor"] = (sensor + 1).ToString();
                _previewTable.Rows.Add(row);
            }
        }

        private UserDataFormatDef CurrentFormat()
        {
            var fmt = new UserDataFormatDef { Name = _txtName.Text.Trim() };
            for (int i = 0; i < Rows; i++)
            {
                string desc = _cboDesc[i].SelectedItem?.ToString() ?? "-";
                int byteNum = (_cboByte[i].SelectedIndex == 1) ? 2 : 1;
                int low = (int)_nudLow[i].Value;
                int high = (int)_nudHigh[i].Value;
                fmt.SetRow(i, desc == "-" ? null : desc, byteNum, low, high);
            }
            return fmt;
        }

        // ── Save & Apply ──────────────────────────────────────────────────────────

        private void btnSave_Click(object sender, EventArgs e)
        {
            var fmt = CurrentFormat();
            if (string.IsNullOrWhiteSpace(fmt.Name))
            {
                MessageBox.Show("Enter a format name.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int count = 0;
            using (var db = new AppDbContext())
            {
                // Upsert: update existing format with the same name, or create new
                var dbFmt = db.Formats.FirstOrDefault(f => f.Name == fmt.Name);
                if (dbFmt == null)
                {
                    dbFmt = new UserDataFormatDef();
                    db.Formats.Add(dbFmt);
                }
                CopyRows(fmt, dbFmt);
                dbFmt.Name = fmt.Name;
                db.SaveChanges(); // ensure Id is assigned before we reference it

                foreach (var s in _sensors)
                {
                    var sensor = db.Sensors.Find(s.RomCode);
                    if (sensor == null) continue;

                    ushort raw = s.RawUserData.HasValue
                        ? (ushort)s.RawUserData.Value
                        : (ushort)((s.BinId << 8) | (s.CableId << 4) | s.SensorNum);

                    var (binVal, cableVal, sensorVal) = dbFmt.Decode(raw);
                    sensor.BinId = (byte)binVal;
                    sensor.CableId = (byte)cableVal;
                    sensor.SensorNum = (byte)sensorVal;
                    sensor.ManualLocation = true;
                    sensor.FormatId = dbFmt.Id;
                    count++;
                }
                db.SaveChanges();
            }

            MessageBox.Show(
                $"{count} sensor(s) updated.\n\n" +
                "Location is now locked — incoming packets will not overwrite it.\n\n" +
                "Use 'Reprogram Selected' in the sensor list to write the new location\n" +
                "into each sensor's EEPROM so future packets decode correctly.",
                "Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static void CopyRows(UserDataFormatDef src, UserDataFormatDef dst)
        {
            dst.R0Desc = src.R0Desc; dst.R0Byte = src.R0Byte; dst.R0Low = src.R0Low; dst.R0High = src.R0High;
            dst.R1Desc = src.R1Desc; dst.R1Byte = src.R1Byte; dst.R1Low = src.R1Low; dst.R1High = src.R1High;
            dst.R2Desc = src.R2Desc; dst.R2Byte = src.R2Byte; dst.R2Low = src.R2Low; dst.R2High = src.R2High;
        }

        private static string FormatRomCode(string rc)
        {
            if (rc == null || rc.Length != 16) return rc ?? "";
            return $"{rc.Substring(0, 4)} {rc.Substring(4, 4)} {rc.Substring(8, 4)} {rc.Substring(12, 4)}";
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserDataEditForm));
            this.SuspendLayout();
            // 
            // UserDataEditForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserDataEditForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }
    }
}
