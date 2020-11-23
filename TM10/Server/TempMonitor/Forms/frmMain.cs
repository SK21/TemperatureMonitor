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
        ReadSensors = 4
    }

    public partial class FormMain : Form
    {
        public int ControlBoxCount;
        public clsDatabase Dbase;
        public int MaxBoxes;

        public PGN25100 ReceiveInfo;
        public bool RecordData;
        public PGN25000 SendInfo;

        public clsSensors Sensors;
        public Tools Tls;
        public UDPComm UDP;

        private DateTime LastDay;
        private DateTime LastReading;
        private DateTime LastReport;

        public clsPackets OutgoingPackets;

        private bool RefreshReadings = false;
        
        private int RecordInterval; // minutes, for sensor readings
        private int SaveInterval;  // hour, for auto report
        public int ReadSensorsDelay = 10; // seconds after Read Sensors command to delay
        public int ControlBoxDelay; // seconds between controlbox messages

                                  
        public FormMain()
        {
            InitializeComponent();

            SendInfo = new PGN25000(this);
            ReceiveInfo = new PGN25100(this);

            Tls = new Tools(this);
            UDP = new UDPComm(this);

            OutgoingPackets = new clsPackets(this);
            Sensors = new clsSensors(this);
            Dbase = new clsDatabase(this);
            Dbase.DBconnected += new clsDatabase.DBconnectedDelegate(Start);
            ReceiveInfo.NewMessage += ReceiveInfo_NewMessage;
        }

        private void ReceiveInfo_NewMessage(object sender, string e)
        {
            WriteEvent(e);
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
            LoadIntervals();
        }

        private void butRefresh_Click(object sender, EventArgs e)
        {
            RefreshReadings = true;
        }

        private void butReports_Click(object sender, EventArgs e)
        {
        }

        private void butSensors_Click(object sender, EventArgs e)
        {
            frmSensors frm = new frmSensors(this);
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void CheckRecord()
        {
            try
            {
                DAO.Recordset RS;
                string SQL = "Select * from tblProps";
                RS = Dbase.DB.OpenRecordset(SQL);
                RecordData = Convert.ToBoolean(RS.Fields["dbRecordData"].Value);
                RS.Close();
            }
            catch (Exception)
            {
            }
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
            CheckRecord();
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

        private void LoadIntervals()
        {
            try
            {
                DAO.Recordset RS;
                string SQL = "select * from tblProps";
                RS = Dbase.DB.OpenRecordset(SQL);
                RecordInterval = (int)(RS.Fields["dbRecordInterval"].Value ?? 0);
                SaveInterval = (int)(RS.Fields["dbSaveInterval"].Value ?? 0);
                ControlBoxDelay = (int)(RS.Fields["dbControlBoxDelay"].Value ?? 0);
                MaxBoxes = (int)(RS.Fields["dbMaxBoxes"].Value ?? 0);
                RS.Close();
            }
            catch (Exception Ex)
            {
                Tls.WriteErrorLog("frmMain: LoadIntervals: " + Ex.Message);
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
            WriteEvent("Database connected.");
            LoadIntervals();
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
            if ((ElapsedMinutes >= RecordInterval) | (RefreshReadings))
            {
                RefreshReadings = false;
                LastReading = DateTime.Now;
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
            CheckRecord();
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
    }
}