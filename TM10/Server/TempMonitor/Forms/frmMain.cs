using System;
using System.Windows.Forms;
using TempMonitor.Classes;
using TempMonitor.Forms;

namespace TempMonitor
{
    public enum PacketType
    {
        AllSensorsReport = 1,
        SingleSensorReport = 2,
        SetUserData = 3,
    }

    public partial class FormMain : Form
    {
        public clsDatabase Dbase;

        public clsPackets OutgoingPackets;
        public PGN25100 ReceiveInfo;
        public PGN25000 SendSensorData;

        public clsSensors Sensors;
        public Tools Tls;
        public UDPComm UDP;

        private DateTime LastDay;
        private DateTime LastReading;
        private DateTime LastReport;

        private int SaveInterval;  // hours, for auto report
        public bool RecordData;     // save to database?
        private int RecordInterval; // minutes, for sensor readings

        public bool ListenOnly;        // listen for PGNs from controlboxes but don't request temperatures
        public int ReadSensorsDelay = 10;   // seconds after Read Sensors command to delay
        public int ControlBoxDelay;     // seconds between controlbox messages

        public int MaxBoxes;
        private bool RefreshReadings = false;

        public int SleepInterval;

        public FormMain()
        {
            InitializeComponent();

            SendSensorData = new PGN25000(this);
            ReceiveInfo = new PGN25100(this);

            Tls = new Tools(this);
            UDP = new UDPComm(this);

            OutgoingPackets = new clsPackets(this);
            Sensors = new clsSensors(this);
            Dbase = new clsDatabase(this);
            Dbase.DBconnected += new clsDatabase.DBconnectedDelegate(Start);
            ReceiveInfo.NewMessage += ReceiveInfo_NewMessage;
        }

        public void WriteEvent(string Message)
        {
            tbEvents.Text += DateTime.Now.ToString() + " - " + Message + "\r\n";
            tbEvents.SelectionStart = tbEvents.Text.Length;
            tbEvents.ScrollToCaret();
            Tls.WriteActivityLog(Message);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout frm = new frmAbout(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void butBins_Click(object sender, EventArgs e)
        {
            frmBins frm = new frmBins(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void butOptions_Click(object sender, EventArgs e)
        {
            frmOptions frm = new frmOptions(this);
            frm.ShowDialog(this);
            frm.Dispose();
            UpdateOptions();
        }

        private void butRefresh_Click(object sender, EventArgs e)
        {
            RefreshReadings = true;
        }

        private void butReports_Click(object sender, EventArgs e)
        {
            frmBinReport frm = new frmBinReport(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void butSensors_Click(object sender, EventArgs e)
        {
            frmSensors frm = new frmSensors(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Tls.PrevInstance())
            {
                Tls.TimedMessageBox("Already Running!");
                this.Close();
            }
            Tls.LoadFormData(this);
            UDP.StartUDPServer();
            if (UDP.isUDPSendConnected)
            {
                WriteEvent("UDP started.");
            }
            else
            {
                WriteEvent("UDP failed to start.");
            }

            if (!Dbase.OpenDatabase()) WriteEvent("Could not open last database.");
            UpdateOptions();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                try
                {
                    Dbase.DB.Close();
                }
                catch (Exception)
                {
                }
                Tls.SaveFormData(this);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNewFile frm = new frmNewFile(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string DBname = "";
            openFileDialog1.InitialDirectory = Tls.DataFolder;
            do
            {
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel) break;
                DBname = openFileDialog1.FileName;
            } while (!Dbase.OpenDatabase(DBname));
            openFileDialog1.Dispose();
        }

        private void ReceiveInfo_NewMessage(object sender, string e)
        {
            WriteEvent(e);
            if (ReceiveInfo.Finished())
            {
                // send reply
                PGN25020 Reply = new PGN25020(this);
                Reply.ControlBoxNumber = ReceiveInfo.ControlBoxNumber();
                clsControlBox Box = new clsControlBox(this);
                Box.Load(0, Reply.ControlBoxNumber);
                Reply.UseSleep = Box.UseSleep;
                Reply.SleepInterval = SleepInterval;
                Reply.ControlBoxCount = Dbase.ControlBoxCount();
                Reply.Send();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Database|*.mdb";
            sfd.DefaultExt = "mdb";
            sfd.AddExtension = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string NewFile = sfd.FileName;
                if (Dbase.SaveAsDB(NewFile, true))
                {
                    WriteEvent("Saved to new file.");
                }
                else
                {
                    Tls.TimedMessageBox("Could not create new file.");
                }
            }
        }

        private void Start()
        {
            UpdateOptions();
            try
            {
                LastDay = Convert.ToDateTime(Tls.GetProperty("LastDay"));
            }
            catch (Exception)
            {
                LastDay = DateTime.Now;
                Tls.SaveProperty("LastDay", LastDay.ToString());
            }
            try
            {
                LastReport = Convert.ToDateTime(Tls.GetProperty("LastReport"));
            }
            catch (Exception)
            {
                LastReport = DateTime.Now;
                Tls.SaveProperty("LastReport", LastDay.ToString());
            }
            timer1.Enabled = true;
            this.Text = "Temperature Monitor [" + Dbase.DBname(true) + "]";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // add packet to for all sensors to report at record interval or refresh command
            int ElapsedMinutes = (int)(DateTime.Now - LastReading).TotalMinutes;
            if (((ElapsedMinutes >= RecordInterval) & !ListenOnly) | (RefreshReadings))
            {
                RefreshReadings = false;
                LastReading = DateTime.Now;

                // issue read command
                PGN25010 Update = new PGN25010(this);
                Update.ReadSensors();

                // create packets for each controlbox
                OutgoingPackets.Add(PacketType.AllSensorsReport);

                WriteEvent("Refreshing.");
            }

            // daily jobs
            if ((DateTime.Now - LastDay).TotalHours > 24)
            {
                Dbase.TrimRecords(Dbase.DB);    // check tblRecords size
                Dbase.CompactDatabase(Dbase.DB.Name);   // compact database
                LastDay = DateTime.Now;
                Tls.SaveProperty("LastDay", LastDay.ToString());
            }

            // auto report
            if ((DateTime.Now - LastReport).TotalHours > SaveInterval)
            {
                // todo save report

                LastReport = DateTime.Now;
                Tls.SaveProperty("LastReport", LastReport.ToString());
            }

            OutgoingPackets.SendNext();
        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOptions frm = new frmOptions(this);
            frm.ShowDialog(this);
            frm.Dispose();
            UpdateOptions();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            frmSensors frm = new frmSensors(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            frmBins frm = new frmBins(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void UpdateOptions()
        {
            try
            {
                DAO.Recordset RS;
                string SQL = "select * from tblProps";
                RS = Dbase.DB.OpenRecordset(SQL);
                RecordInterval = Dbase.FieldToInt(RS, "dbRecordInterval");
                SaveInterval = Dbase.FieldToInt(RS, "dbSaveInterval");
                ControlBoxDelay = Dbase.FieldToInt(RS, "dbControlBoxDelay");
                MaxBoxes = Dbase.FieldToInt(RS, "dbMaxBoxes");
                RecordData = Convert.ToBoolean(RS.Fields["dbRecordData"].Value);
                ListenOnly = Convert.ToBoolean(RS.Fields["dbListenOnly"].Value);
                SleepInterval = Dbase.FieldToInt(RS, "dbSleepInterval");
                RS.Close();
            }
            catch (Exception Ex)
            {
                Tls.WriteErrorLog("frmMain: UpdateOptions: " + Ex.Message);
            }
        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            frmControlBoxes frm = new frmControlBoxes(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void butControlBoxes_Click(object sender, EventArgs e)
        {
            frmControlBoxes frm = new frmControlBoxes(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            frmBinReport frm = new frmBinReport(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Name = Dbase.DBname(true);
            if (Tls.BackupFile(Name))
            {
                WriteEvent("File backed up.");
            }
            else
            {
                WriteEvent("Failed to backup File.");
            }
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult Result;
            Result = MessageBox.Show("Confirm restore File.", "Restore File", MessageBoxButtons.YesNo);
            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                string Name = Dbase.DBname(true);
                if (Tls.RestoreFile(Name))
                {
                    WriteEvent("File restored.");
                }
                else
                {
                    WriteEvent("Failed to Restore File.");
                }
            }
        }

        private void indexToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}