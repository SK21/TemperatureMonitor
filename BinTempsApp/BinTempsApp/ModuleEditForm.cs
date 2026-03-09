using System;
using System.Linq;
using System.Windows.Forms;
using BinTempsApp.Models;

namespace BinTempsApp
{
    public partial class ModuleEditForm : Form
    {
        private readonly Module _module;

        public ModuleEditForm(Module module)
        {
            _module = module;
            InitializeComponent();

            lblMacValue.Text = module.MacAddress;
            txtName.Text = module.Name ?? "";
            nudId.Value = module.ModuleId;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            if (name.Length > 10)
            {
                MessageBox.Show("Name must be 10 characters or fewer.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte newId = (byte)nudId.Value;

            // Save to DB and fire ModuleUpdated event so the grid refreshes
            AppServices.ModuleService.SaveModuleLocally(_module.MacAddress, newId, name);

            // Send unicast to the module's last known IP (avoids broadcast routing issues)
            byte[] macBytes = _module.MacAddress
                .Split(':')
                .Select(h => Convert.ToByte(h, 16))
                .ToArray();
            AppServices.ModuleService.RegisterModule(macBytes, newId, name, _module.LastKnownIp);

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
