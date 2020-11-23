using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

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

        public bool CellCheckedValue(int Row, String Col)
        {
            bool Result = false;
            try
            {
                string Val = DGV.Rows[Row].Cells[Col].Value.ToString();
                Result = (Val == "True");
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmSensors:CellCheckedValue " + ex.Message);
            }
            return Result;
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
        }

        private void btReload_Click(object sender, EventArgs e)
        {
            LoadData();
            LoadBins();
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
                    //clsSensor Sen = Sensors.Item((short)CellIntValue(CurrentRow, "ID"));
                    clsSensor Sen = Sensors.Item(Convert.ToInt16(NV(0)));

                    short Rslt = 0;
                    short.TryParse(tbCable.Text, out Rslt);
                    Sen.CableID = Rslt;

                    Rslt = 0;
                    short.TryParse(tbSensor.Text, out Rslt);
                    Sen.SensorID = Rslt;

                    float Flt = 0;
                    float.TryParse(tbOffset.Text, out Flt);
                    Sen.OffSet = Flt;

                    Rslt = 0;
                    short.TryParse(cbBin.Text, out Rslt);
                    Sen.BinNum = Rslt;

                    Sen.Enabled = ckEnabled.Checked;

                    Sen.Save();
                    LoadData();
                    DGV.Rows[CurrentRow].Selected = true;
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

        private void cbBin_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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

        private void DGV_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (butSaveEdit.Text == "Save")
            {
                butSaveEdit.PerformClick();
            }
        }

        private void frmSensors_Load(object sender, EventArgs e)
        {
            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 255, 255);
            DGV.EnableHeadersVisualStyles = false;

            tbEvents.BackColor = DGV.DefaultCellStyle.BackColor;

            LoadData();
            LoadBins();
            UpdateDisplay();
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void LoadBins()
        {
            Updating = true;
            try
            {
                dsBins.Clear();

                foreach (clsStorage Stor in Bins.Items)
                {
                    DataRow Rw = dsBins.Tables[0].NewRow();
                    Rw[0] = Stor.ID;
                    Rw[1] = Stor.Number;
                    dsBins.Tables[0].Rows.Add(Rw);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmSensors:LoadBins " + ex.Message);
            }
            Updating = false;
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
                    Rw[2] = Sen.CableID;
                    Rw[3] = Sen.SensorID;
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

        private void SetSelectedBin(int BinNum)
        {
            bool Found = false;
            for (int i = 0; i < cbBin.Items.Count; i++)
            {
                cbBin.SelectedIndex = i;
                if (Convert.ToInt32(cbBin.Text) == BinNum)
                {
                    Found = true;
                    break;
                }
            }
            if (!Found) cbBin.SelectedIndex = -1;
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

                tbSensor.Text = DGV.Rows[CurrentRow].Cells[4].Value.ToString();
                tbCable.Text = DGV.Rows[CurrentRow].Cells[3].Value.ToString();

                bool Enabled = false;
                bool.TryParse(DGV.Rows[CurrentRow].Cells[5].Value.ToString(), out Enabled);
                ckEnabled.Checked = Enabled;

                float Offset = 0;
                float.TryParse(DGV.Rows[CurrentRow].Cells[6].Value.ToString(), out Offset);
                tbOffset.Text = Offset.ToString("N1");

                int BinNum = 0;
                int.TryParse(DGV.Rows[CurrentRow].Cells[1].Value.ToString(), out BinNum);
                SetSelectedBin(BinNum);

                lbSelected.Text = DGV.Rows[CurrentRow].Cells[9].Value.ToString();

                Updating = false;
            }
        }
    }
}