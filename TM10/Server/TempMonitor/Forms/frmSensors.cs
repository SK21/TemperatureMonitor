using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using DGVPrinterHelper;

namespace TempMonitor.Forms
{
    public partial class frmSensors : Form
    {
        private clsStorages Bins;
        private int CurrentRow;
        private FormMain mf;
        private clsSensors Sensors;
        private bool Updating;

        public frmSensors(FormMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            Bins = new clsStorages(mf);
            Bins.Load();

            Sensors = new clsSensors(mf);
            Sensors.Load();
            mf.ReceiveInfo.NewMessage += ReceiveInfo_NewMessage;
        }

        public void WriteEvent(string Message, bool ShowTime = true)
        {
            if (ShowTime)
            {
                tbEvents.Text += DateTime.Now.ToString() + " -";
            }
            tbEvents.Text += " " + Message + "\r\n";
            tbEvents.SelectionStart = tbEvents.Text.Length;
            tbEvents.ScrollToCaret();
        }

        private void btCheck_Click(object sender, EventArgs e)
        {
            WriteEvent("Checking Sensor...");
            CheckSensor();
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
            DGVPrinter printer = new DGVPrinter();
            printer.Title = "Sensors";
            //printer.SubTitle = "An Easy to Use DataGridView Printing Object";
            printer.SubTitleFormatFlags = StringFormatFlags.LineLimit |
            StringFormatFlags.NoClip;
            printer.PageNumbers = true;
            printer.PageNumberInHeader = false;
            printer.ColumnWidth = DGVPrinter.ColumnWidthSetting.Porportional;
            printer.HeaderCellAlignment = StringAlignment.Near;
            //printer.Footer = "Your Company Name Here";
            printer.FooterSpacing = 15;
            printer.PrintPreviewDataGridView(DGV);
        }

        private void btReload_Click(object sender, EventArgs e)
        {
            LoadData();
            UpdateDisplay();
        }

        private void btWrite_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] Adr = new byte[8];
                Adr = mf.Tls.ConvertAddressString(DGV.Rows[CurrentRow].Cells[9].Value.ToString());

                int Bin = Convert.ToInt32(NV(2));
                int Cable = Convert.ToInt32(NV(3));
                int Sensor = Convert.ToInt32(NV(4));

                int UserData = mf.Tls.ConvertToUserData(Bin, Cable, Sensor);

                mf.OutgoingPackets.Add(PacketType.SetUserData, Adr, UserData);
                timer1.Enabled = true;  // display sensor values after timer delay
                WriteEvent("Writing to sensor...");
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmSensors:btWrite_Click " + ex.Message);
                mf.Tls.TimedMessageBox(ex.Message);
            }
        }

        private void butCancelEdit_Click(object sender, EventArgs e)
        {
            SetButtons(false);
            UpdateDisplay();
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Confirm Delete Sensor?", "Delete Record", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    short ID = Convert.ToInt16(NV(0));
                    Sensors.Delete(ID);
                }
                catch (Exception ex)
                {
                    WriteEvent(ex.Message, false);
                }
                LoadData();
            }
        }

        private void butSaveEdit_Click(object sender, EventArgs e)
        {
            if (butSaveEdit.Text == "Close")
            {
                this.Close();
            }
            else
            {
                try
                {
                    clsSensor Sen = Sensors.Item(Convert.ToInt16(NV(0)));

                    byte Rslt = 0;
                    byte.TryParse(tbCable.Text, out Rslt);
                    Sen.CableNum = Rslt;

                    Rslt = 0;
                    byte.TryParse(tbSensor.Text, out Rslt);
                    Sen.SensorNum = Rslt;

                    float Flt = 0;
                    float.TryParse(tbOffset.Text, out Flt);
                    Sen.OffSet = Flt;

                    Rslt = 0;
                    byte.TryParse(tbBin.Text, out Rslt);
                    Sen.BinNum = Rslt;

                    Sen.Enabled = ckEnabled.Checked;

                    Sen.Save();
                    LoadData();

                    CurrentRow = mf.Tls.FindRecord(DGV, 0, Sen.ID);
                    DGV.CurrentCell = DGV[2, CurrentRow];
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException)
                    {
                        mf.Tls.TimedMessageBox(ex.Message);
                    }
                    else
                    {
                        WriteEvent(ex.Message, false);
                        mf.Tls.WriteErrorLog("frmSensors:butSaveEdit_Click " + ex.Message);
                    }
                }
                UpdateDisplay();
                SetButtons(false);
            }
        }

        private void CheckSensor()
        {
            byte[] Adr = new byte[8];
            Adr = mf.Tls.ConvertAddressString(DGV.Rows[CurrentRow].Cells[9].Value.ToString());

            mf.OutgoingPackets.Add(PacketType.SingleSensorReport, Adr);
        }

        private void ckEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void DGV_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV.CurrentRow.Index != CurrentRow & !Updating)
            {
                CurrentRow = DGV.CurrentRow.Index;
                UpdateDisplay();
            }
        }

        private void frmSensors_Load(object sender, EventArgs e)
        {
            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 255, 255);
            DGV.EnableHeadersVisualStyles = false;

            tbEvents.BackColor = DGV.DefaultCellStyle.BackColor;

            LoadData();
            UpdateDisplay();
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void LoadData()
        {
            Updating = true;
            try
            {
                dataSet1.Clear();

                foreach (clsSensor Sen in Sensors.Items)
                {
                    DataRow Rw = dataSet1.Tables[0].NewRow();
                    Rw[0] = Sen.ID;
                    Rw[1] = Sen.BinNum;
                    Rw[2] = Sen.CableNum;
                    Rw[3] = Sen.SensorNum;
                    Rw[4] = Sen.Enabled;
                    Rw[5] = Sen.OffSet;
                    Rw[6] = Sen.LastTemp();
                    Rw[7] = Sen.LastTime();
                    Rw[8] = Sen.SensorAddress;
                    Rw[9] = Sen.BinDescription(true);

                    dataSet1.Tables[0].Rows.Add(Rw);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmSensors:LoadData " + ex.Message);
            }
            Updating = false;
        }

        private string NV(int Cell, bool IsNumber = true, DataGridViewRow RW = null)
        {
            if (RW == null) RW = DGV.Rows[CurrentRow];

            string Val = "";
            try
            {
                Val = RW.Cells[Cell].Value.ToString();
            }
            catch (Exception)
            {
            }
            if ((Val == "") & IsNumber) Val = "0";
            return Val;
        }

        private void ReceiveInfo_NewMessage(object sender, string e)
        {
            WriteEvent(e);
        }

        private void SetButtons(bool Edited)
        {
            if (!Updating)
            {
                if (Edited)
                {
                    butCancelEdit.Enabled = true;
                    butSaveEdit.Text = "Save";
                    btWrite.Enabled = false;
                    btReload.Enabled = false;
                    btCheck.Enabled = false;
                    butDelete.Enabled = false;
                    btPrint.Enabled = false;
                }
                else
                {
                    butCancelEdit.Enabled = false;
                    butSaveEdit.Text = "Close";
                    btWrite.Enabled = true;
                    btReload.Enabled = true;
                    btCheck.Enabled = true;
                    butDelete.Enabled = true;
                    btPrint.Enabled = true;
                }
            }
        }

        private void tbCable_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbOffset_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSensor_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            WriteEvent("Checking sensor...");
            CheckSensor();
        }

        private void UpdateDisplay()
        {
            if (DGV.CurrentRow != null)
            {
                Updating = true;

                tbBin.Text = DGV.Rows[CurrentRow].Cells[1].Value.ToString();
                tbSensor.Text = DGV.Rows[CurrentRow].Cells[4].Value.ToString();
                tbCable.Text = DGV.Rows[CurrentRow].Cells[3].Value.ToString();

                bool Enabled = false;
                bool.TryParse(DGV.Rows[CurrentRow].Cells[5].Value.ToString(), out Enabled);
                ckEnabled.Checked = Enabled;

                float Offset = 0;
                float.TryParse(DGV.Rows[CurrentRow].Cells[6].Value.ToString(), out Offset);
                tbOffset.Text = Offset.ToString("N1");

                lbSelected.Text = DGV.Rows[CurrentRow].Cells[9].Value.ToString();

                Updating = false;
            }
        }

        private void tbBin_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }
    }
}