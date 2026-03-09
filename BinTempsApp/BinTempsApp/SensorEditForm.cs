using System;
using System.Windows.Forms;
using BinTempsApp.Models;

namespace BinTempsApp
{
    public partial class SensorEditForm : Form
    {
        private readonly Sensor _sensor;

        public SensorEditForm(Sensor sensor)
        {
            _sensor = sensor;
            InitializeComponent();

            lblRomCodeValue.Text = sensor.RomCode;
            lblModuleValue.Text  = sensor.Module?.Name ?? sensor.ModuleMac ?? "-";
            lblLocationValue.Text = $"Bin {sensor.BinId + 1} / Cable {sensor.CableId + 1} / Sensor {sensor.SensorNum + 1}";

            txtLabel.Text       = sensor.Label ?? "";
            nudMaxTemp.Value    = (decimal)sensor.MaxTemp;
            nudOffset.Value     = (decimal)sensor.Offset;
            chkEnabled.Checked  = sensor.Enabled;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string label   = txtLabel.Text.Trim();
            float maxTemp  = (float)nudMaxTemp.Value;
            float offset   = (float)nudOffset.Value;
            bool enabled   = chkEnabled.Checked;

            var svc = AppServices.SensorService;
            svc.UpdateLabel(_sensor.RomCode, label);
            svc.UpdateMaxTemp(_sensor.RomCode, maxTemp);
            svc.UpdateOffset(_sensor.RomCode, offset);
            svc.SetEnabled(_sensor.RomCode, enabled);

            // Update the local copy so the caller can read updated values
            _sensor.Label   = label;
            _sensor.MaxTemp = maxTemp;
            _sensor.Offset  = offset;
            _sensor.Enabled = enabled;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
