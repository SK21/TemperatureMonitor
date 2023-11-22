using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TempMonitor.Forms;

namespace TempMonitor
{
    public class clsTools
    {
        private static Hashtable ht = new Hashtable();
        private string BackupDir;
        private string cAppName = "TemperatureMonitor";
        private string cDBtype = "Temps";
        private int cDBversion = 1;
        private string cVersionDate = "22-Nov-2023";
        private string DataDir;
        private frmMain mf;
        private string PropertiesFile;
        private string SettingsDir;

        public clsTools(frmMain cf)
        {
            mf = cf;
            CheckFolders();
        }

        public string AppName
        { get { return cAppName; } }

        public string DataFolder
        {
            get { return DataDir; }
            set
            {
                if (Directory.Exists(value))
                {
                    DataDir = value;
                    SaveProperty("DataDir", DataDir);
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(value);
                        DataDir = value;
                        SaveProperty("DataDir", DataDir);
                    }
                    catch (Exception ex)
                    {
                        WriteErrorLog("clsTools: Set DataFolder: " + ex.Message);
                        ShowHelp("Could not create data folder:  " + DataDir);
                    }
                }
            }
        }

        public int DBversion
        { get { return cDBversion; } }

        public string SettingsFolder
        { get { return SettingsDir; } }

        public string VersionDate
        { get { return cVersionDate; } }

        public bool BackupFile(string FileName)
        {
            FileName += ".mdf";
            string Destination = BackupDir + "\\" + FileName;
            string Source = DataDir + "\\" + FileName;
            bool Result = false;
            try
            {
                if (File.Exists(Destination)) File.Delete(Destination);
                File.Copy(Source, Destination);
                Result = true;
            }
            catch (Exception ex)
            {
                WriteErrorLog("clsTools:BackupFile" + ex.Message);
            }
            return Result;
        }

        public byte BinNumFromUserData(short UD)
        {
            return (byte)(UD >> 8); // shift right to lower 8 bits
        }

        public byte CableNumFromUserData(short UD)
        {
            int Tmp = (UD & 240) >> 4;  // remove top 8 and lower 4 bits, 0000 0000 1111 0000
            return (byte)Tmp;
        }

        public byte[] ConvertAddressString(string Adr)
        {
            // converts address string to a byte array
            // expects address hex representation ex: "28 29 91 3C 07 00 00 64"

            string[] Parts;
            byte[] Result = new byte[8];

            Parts = Adr.Split(separator: ' ');
            if (Parts.Length == 8)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (!byte.TryParse(Parts[i], System.Globalization.NumberStyles.HexNumber, null, out Result[i]))
                    {
                        // error in parsing
                        for (int j = 0; j < 8; j++)
                        {
                            Result[j] = 0;
                        }
                        break;
                    }
                }
            }
            return Result;
        }

        public short ConvertToUserData(int BinNum, int CableNum, int SensorNum)
        {
            // BinNum, byte 1, 0-255
            // CableNum, left 4 bits of byte 2, 0-15
            // SensorNum, right 4 bits of byte 2, 0-15

            if (BinNum < 0 | BinNum > 255 | SensorNum < 0 | SensorNum > 15 | CableNum < 0 | CableNum > 15) throw new ArgumentException("Invalid number.");
            byte byte2 = (byte)((CableNum << 4) | SensorNum);
            return (short)((BinNum << 8) | byte2);
        }

        public bool DatabaseFound(string Path)
        {
            bool Result = false;
            int dbVersion = 0;
            string dbType = "";

            try
            {
                string ConString = "Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30;AttachDbFilename=" + Path;

                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SQL = "Select * from tblProps";
                    using (SqlCommand cmd = new SqlCommand(SQL, con))
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            dbVersion = reader.GetInt32(0);
                            dbType = reader.GetString(1);
                        }
                    }
                }

                //if (dbVersion == cDBversion && dbType == cDBtype)
                //{
                //    Properties.Settings.Default.connString = ConString;
                //    Result = true;
                //}

                Result = (dbVersion == cDBversion && dbType == cDBtype);
            }
            catch (Exception ex)
            {
                WriteErrorLog("clsTools: DatabaseFound: " + ex.Message);
            }
            return Result;
        }

        public string GetProperty(string Key)
        {
            Key = Key.ToLower();
            string Prop = "";
            if (ht.Contains(Key)) Prop = ht[Key].ToString();
            return Prop;
        }

        public string HexAddressFromBytes(byte[] addr)
        {
            string Result = "";
            if (addr.Length == 8)
            {
                // converts array of bytes to hex representation ex: "28 29 91 3C 07 00 00 64"
                Result = BitConverter.ToString(addr).Replace("-", " ");
            }
            return Result;
        }

        public bool IsOnScreen(Form form)
        {
            // Create rectangle
            Rectangle formRectangle = new Rectangle(form.Left, form.Top, form.Width, form.Height);

            // Test
            return Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(formRectangle));
        }

        public void LoadFormData(Form Frm)
        {
            int Leftloc = 0;
            int.TryParse(GetProperty(Frm.Name + ".Left"), out Leftloc);
            Frm.Left = Leftloc;

            int Toploc = 0;
            int.TryParse(GetProperty(Frm.Name + ".Top"), out Toploc);
            Frm.Top = Toploc;

            if (!IsOnScreen(Frm))
            {
                Frm.Left = 0;
                Frm.Top = 0;
            }
        }

        public bool PrevInstance()
        {
            string PrsName = Process.GetCurrentProcess().ProcessName;
            Process[] All = Process.GetProcessesByName(PrsName); //Get the name of all processes having the same name as this process name
            if (All.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RestoreFile(string FileName)
        {
            FileName += ".mdf";
            string Destination = DataDir + "\\" + FileName;
            string Source = BackupDir + "\\" + FileName;
            bool Result = false;
            try
            {
                if (File.Exists(Destination)) File.Delete(Destination);
                File.Copy(Source, Destination);
                Result = true;
            }
            catch (Exception ex)
            {
                WriteErrorLog("clsTools:RestoreFile" + ex.Message);
            }
            return Result;
        }

        public void SaveFormData(Form Frm)
        {
            SaveProperty(Frm.Name + ".Left", Frm.Left.ToString());
            SaveProperty(Frm.Name + ".Top", Frm.Top.ToString());
        }

        public void SaveProperty(string Key, string Value)
        {
            Key = Key.ToLower();
            Value = Value.ToLower();
            bool Changed = false;
            if (ht.Contains(Key))
            {
                if (!ht[Key].ToString().Equals(Value))
                {
                    ht[Key] = Value;
                    Changed = true;
                }
            }
            else
            {
                ht.Add(Key, Value);
                Changed = true;
            }
            if (Changed) SaveProperties();
        }

        public byte SensorNumFromUserData(short UD)
        {
            return (byte)(UD & 15); // remove top 12 bits, 0000 0000 0000 1111
        }

        public void ShowHelp(string Message, string Title = "Help",
            int timeInMsec = 20000, bool LogError = false, bool Modal = false)
        {
            var Hlp = new frmHelp(mf, Message, Title, timeInMsec);
            if (Modal)
            {
                Hlp.ShowDialog();
            }
            else
            {
                Hlp.Show();
            }

            if (LogError) WriteErrorLog(Message);
        }

        public void WriteActivityLog(string Message)
        {
            try
            {
                string FileName = SettingsDir + "\\Activity Log.txt";
                TrimFile(FileName);
                File.AppendAllText(FileName, DateTime.Now.ToString() + "  -  " + Message + "\r\n\r\n");
            }
            catch (Exception ex)
            {
                WriteErrorLog("clsTools: WriteActivityLog: " + ex.Message);
            }
        }

        public void WriteErrorLog(string strErrorText)
        {
            try
            {
                string FileName = SettingsDir + "\\Error Log.txt";
                TrimFile(FileName);
                File.AppendAllText(FileName, DateTime.Now.ToString() + "  -  " + strErrorText + "\r\n\r\n");
            }
            catch (Exception ex)
            {
                ShowHelp("Error in WriteErrorLog:  " + ex.Message);
            }
        }

        private void CheckFolders()
        {
            try
            {
                // BackupDir
                BackupDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + cAppName + "\\Backup";
                if (!Directory.Exists(BackupDir)) Directory.CreateDirectory(BackupDir);

                // SettingsDir
                SettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + cAppName;
                if (!Directory.Exists(SettingsDir)) Directory.CreateDirectory(SettingsDir);

                // properties file
                PropertiesFile = SettingsDir + "\\Properties.txt";
                if (!File.Exists(PropertiesFile)) File.Create(PropertiesFile).Dispose();

                LoadProperties(PropertiesFile);

                // Data directory
                DataDir = GetProperty("DataDir");
                if (!Directory.Exists(DataDir))
                {
                    DataDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + cAppName + "\\Data";
                    if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
                    if (!File.Exists(DataDir + "\\Example.mdf")) File.WriteAllBytes(DataDir + "\\Example.mdf", Properties.Resources.Example);
                    SaveProperty("DataDir", DataDir);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: CheckFolders: " + ex.Message);
            }
        }

        private void LoadProperties(string path)
        {
            // property:  key=value  ex: "LastFile=Main.mdb"
            try
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (line.Contains("=") && !String.IsNullOrEmpty(line.Split('=')[0]) && !String.IsNullOrEmpty(line.Split('=')[1]))
                    {
                        string[] splitText = line.Split('=');
                        ht.Add(splitText[0].ToLower(), splitText[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("clsTools: LoadProperties: " + ex.Message);
            }
        }

        private void SaveProperties()
        {
            string[] NewLines = new string[ht.Count];
            int i = -1;
            foreach (DictionaryEntry Pair in ht)
            {
                i++;
                NewLines[i] = Pair.Key.ToString() + "=" + Pair.Value.ToString();
            }
            if (i > -1) File.WriteAllLines(PropertiesFile, NewLines);
        }

        private void TrimFile(string FileName, int MaxSize = 25000)
        {
            try
            {
                if (File.Exists(FileName))
                {
                    long FileSize = new FileInfo(FileName).Length;
                    if (FileSize > MaxSize)
                    {
                        // trim file
                        string[] Lines = File.ReadAllLines(FileName);
                        int Len = (int)Lines.Length;
                        int St = (int)(Len * .1); // skip first 10% of old lines
                        string[] NewLines = new string[Len - St];
                        Array.Copy(Lines, St, NewLines, 0, Len - St);
                        File.Delete(FileName);
                        File.AppendAllLines(FileName, NewLines);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("clsTools: TrimFile: " + ex.Message);
            }
        }
    }
}