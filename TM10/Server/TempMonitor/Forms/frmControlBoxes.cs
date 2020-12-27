using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TempMonitor
{
    public partial class frmControlBoxes : Form
    {
        private clsControlBoxes Boxes;
        private int CurrentRow;
        private FormMain mf;
        private bool NewRecord;
        private bool Updating;

        public frmControlBoxes(FormMain CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            Boxes = new clsControlBoxes(mf);
            Boxes.Load();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewRecord = true;
            Updating = true;

            tbNumber.Text = "";
            tbDescription.Text = "";
            ckSleep.Checked = false;

            Updating = false;
            SetButtons(true);
        }

        private void butCancelEdit_Click(object sender, EventArgs e)
        {
            SetButtons(false);
            UpdateDisplay();
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Confirm Delete Controlbox?", "Delete Record", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    byte ID = Convert.ToByte(DGV.Rows[CurrentRow].Cells[0].Value);
                    Boxes.Delete(ID);
                }
                catch (Exception ex)
                {
                    mf.Tls.TimedMessageBox(ex.Message);
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
                    clsControlBox Box;
                    if (DGV.Rows.Count == 0 | NewRecord)
                    {
                        // first record
                        Box = Boxes.Add();
                    }
                    else
                    {
                        Box = Boxes.Item(Convert.ToByte(DGV.Rows[CurrentRow].Cells[0].Value));
                    }

                    byte Result = 0;
                    byte.TryParse(tbNumber.Text, out Result);
                    Box.BoxID = Result;

                    Box.UseSleep = ckSleep.Checked;
                    Box.Description = tbDescription.Text;

                    Box.Save();
                    LoadData();

                    CurrentRow = mf.Tls.FindRecord(DGV, 1, Box.BoxID);
                    DGV.CurrentCell = DGV[1, CurrentRow];
                }
                catch (Exception ex)
                {
                    mf.Tls.TimedMessageBox(ex.Message);
                    mf.Tls.WriteErrorLog("frmControlBoxes:butSaveEdit_Click " + ex.Message);
                }
            }
            UpdateDisplay();
            SetButtons(false);
            NewRecord = false;
        }

        private void ckSleep_CheckedChanged(object sender, EventArgs e)
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

        private void frmControlBoxes_Load(object sender, EventArgs e)
        {
            DGV.BackgroundColor = DGV.DefaultCellStyle.BackColor;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 255, 255);
            DGV.EnableHeadersVisualStyles = false;

            LoadData();
            UpdateDisplay();
            DGV.CurrentCell = DGV.FirstDisplayedCell;
        }

        private void LoadData()
        {
            Updating = true;
            try
            {
                dataSet1.Clear();

                foreach (clsControlBox Box in Boxes.Items)
                {
                    DataRow Rw = dataSet1.Tables[0].NewRow();
                    Rw[0] = Box.ID;
                    Rw[1] = Box.BoxID;
                    Rw[2] = Box.UseSleep;
                    Rw[3] = Box.Description;
                    dataSet1.Tables[0].Rows.Add(Rw);
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmControlBoxes:LoadData " + ex.Message);
            }
            Updating = false;
        }

        private void SetButtons(bool Edited)
        {
            if (!Updating)
            {
                if (Edited)
                {
                    butCancelEdit.Enabled = true;
                    butSaveEdit.Text = "Save";
                    DGV.Enabled = false;
                    btPrint.Enabled = false;
                    butDelete.Enabled = false;
                    btnNew.Enabled = false;
                }
                else
                {
                    butCancelEdit.Enabled = false;
                    butSaveEdit.Text = "Close";
                    DGV.Enabled = true;
                    btPrint.Enabled = true;
                    butDelete.Enabled = true;
                    btnNew.Enabled = true;
                }
            }
        }

        private void tbDescription_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbNumber_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void UpdateDisplay()
        {
            if (DGV.CurrentRow != null)
            {
                Updating = true;

                tbNumber.Text = mf.Tls.NV(DGV.CurrentRow, 1, true);
                ckSleep.Checked = mf.Tls.StringToBool(mf.Tls.NV(DGV.CurrentRow, 2));
                tbDescription.Text = mf.Tls.NV(DGV.CurrentRow, 3);


                Updating = false;
            }
        }

        private void btPrint_Click(object sender, EventArgs e)
        {

        }
    }
}