using System;
using System.Windows.Forms;
using BinWatch.Models;

namespace BinWatch
{
    public partial class SensorEditForm : Form
    {
        private readonly Sensor _sensor;

        public SensorEditForm(Sensor sensor)
        {
            _sensor = sensor;
            InitializeComponent();
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            lblRomCodeValue.Text = sensor.RomCode;
            lblModuleValue.Text  = sensor.Module?.Name ?? sensor.ModuleMac ?? "-";

            // Location — 1-indexed display, 0-indexed storage
            nudBin.Value       = sensor.BinId + 1;
            nudCable.Value     = sensor.CableId + 1;
            nudSensorNum.Value = sensor.SensorNum + 1;

            txtLabel.Text      = sensor.Label ?? "";
            nudMaxTemp.Value   = (decimal)sensor.MaxTemp;
            nudOffset.Value    = (decimal)sensor.Offset;
            chkEnabled.Checked = sensor.Enabled;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            byte binId     = (byte)(nudBin.Value - 1);
            byte cableId   = (byte)(nudCable.Value - 1);
            byte sensorNum = (byte)(nudSensorNum.Value - 1);
            string label   = txtLabel.Text.Trim();
            float maxTemp  = (float)nudMaxTemp.Value;
            float offset   = (float)nudOffset.Value;
            bool enabled   = chkEnabled.Checked;

            var svc = AppServices.SensorService;
            svc.UpdateLocation(_sensor.RomCode, binId, cableId, sensorNum);
            svc.UpdateLabel(_sensor.RomCode, label);
            svc.UpdateMaxTemp(_sensor.RomCode, maxTemp);
            svc.UpdateOffset(_sensor.RomCode, offset);
            svc.SetEnabled(_sensor.RomCode, enabled);

            // Write user data to sensor EEPROM if active (not passive mode) and
            // module is known so we can address the command
            if (!AppConfig.PassiveMode && _sensor.Module != null)
            {
                byte[] romBytes = HexToBytes(_sensor.RomCode);
                svc.SendUserData(_sensor.Module.ModuleId, romBytes, binId, cableId, sensorNum);
            }

            // Update the local copy so the caller can read updated values
            _sensor.BinId     = binId;
            _sensor.CableId   = cableId;
            _sensor.SensorNum = sensorNum;
            _sensor.Label     = label;
            _sensor.MaxTemp   = maxTemp;
            _sensor.Offset    = offset;
            _sensor.Enabled   = enabled;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
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
