using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace TempMonitor
{
    public class Tools
    {
        private static Hashtable ht = new Hashtable();
        private readonly FormMain mf;
        private string cAppName = "TemperatureMonitor";
        private int cCurrentDBversion = 100;
        private string cVersionDate = "23-Jan-2021";
        private string DataDir;
        private string PropertiesFile;
        private string SettingsDir;
        private string BackupDir;

        public Tools(FormMain CallingForm)
        {
            mf = CallingForm;
            CheckFolders();
        }

        public string AppName { get { return cAppName; } }

        public int CurrentDBversion { get { return cCurrentDBversion; } }

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
                        WriteErrorLog("Tools: Set DataFolder: " + ex.Message);
                        TimedMessageBox("Could not create data folder.", DataDir);
                    }
                }
            }
        }

        public string SettingsFolder { get { return SettingsDir; } }
        public string VersionDate { get { return cVersionDate; } }

        public string GetProperty(string Key)
        {
            Key = Key.ToLower();
            string Prop = "";
            if (ht.Contains(Key)) Prop = ht[Key].ToString();
            return Prop;
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

        public int StringToInt(string S)
        {
            if (decimal.TryParse(S, out decimal tmp))
            {
                return (int)tmp;
            }
            return 0;
        }

        public void TimedMessageBox(string s1, string s2 = "", int timeout = 3000)
        {
            var form = new FormTimedMessage(s1, s2, timeout);
            form.Show();
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
                WriteErrorLog("Tools: WriteActivityLog: " + ex.Message);
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
                TimedMessageBox("Error in WriteErrorLog", ex.Message);
            }
        }

        private void CheckFolders()
        {
            try
            {
                // BackupDir
                BackupDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + cAppName + "\\Backup";
                if (!Directory.Exists(BackupDir)) Directory.CreateDirectory(BackupDir);

                // SettingsDir
                SettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + cAppName + "\\Settings";
                if (!Directory.Exists(SettingsDir)) Directory.CreateDirectory(SettingsDir);

                // properties file
                PropertiesFile = SettingsDir + "\\Properties.txt";
                if (!File.Exists(PropertiesFile)) File.Create(PropertiesFile).Dispose();

                LoadProperties(PropertiesFile);

                // Data directory
                DataDir = GetProperty("DataDir");
                if (!Directory.Exists(DataDir))
                {
                    DataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + cAppName;
                    if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
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
                WriteErrorLog("Tools: LoadProperties: " + ex.Message);
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
                WriteErrorLog("Tools: TrimFile: " + ex.Message);
            }
        }

        public double Temperature(byte[] TempBytes)
        {
            // expects 2 bytes that are 10 times actual temperature
            if (TempBytes.Length == 2)
            {
                return (double)((TempBytes[0] << 8 | TempBytes[1]) / 10);
            }
            return -127;
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

        public byte BinNumFromUserData(int UD)
        {
            return (byte)(UD >> 8); // shift right to lower 8 bits
        }

        public byte CableNumFromUserData(int UD)
        {
            int Tmp = (UD & 240) >> 4;  // remove top 8 and lower 4 bits, 0000 0000 1111 0000
            return (byte)Tmp;
        }

        public byte SensorNumFromUserData(int UD)
        {
            return (byte)(UD & 15); // remove top 12 bits, 0000 0000 0000 1111
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


        public bool StringToBool(string Val)
        {
            return (Val == "1" | Val == "true" | Val == "True");
        }

        public string NV(DataGridViewRow RW, int Cell, bool ReturnNumber = false)
        {
            //Null value of cell
            string Val = "";
            try
            {
                Val = RW.Cells[Cell].Value.ToString();
            }
            catch (Exception)
            {

            }

            if (ReturnNumber & (Val == "")) Val = "0";

            return Val;
        }

        public int FindRecord(DataGridView DGV, int Cell, int Key)
        {
            try
            {
                foreach (DataGridViewRow RW in DGV.Rows)
                {
                    if (Convert.ToInt32(RW.Cells[Cell].Value) == Key)
                    {
                        return RW.Index;
                    }
                }
            }
            catch (Exception)
            {

            }
            return 0;
        }

        public bool BackupFile(string FileName)
        {
            FileName += ".mdb";
            string Destination = BackupDir + "\\" + FileName;
            string Source = DataDir + "\\" + FileName;
            bool Result = false;
            mf.Dbase.DB.Close();
            try
            {
                if (File.Exists(Destination)) File.Delete(Destination);
                File.Copy(Source, Destination);
                Result = true;
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools:BackupFile" + ex.Message);
            }
            mf.Dbase.OpenDatabase();
            return Result;
        }

        public bool RestoreFile(string FileName)
        {
            FileName += ".mdb";
            string Destination = DataDir + "\\" + FileName;
            string Source = BackupDir + "\\" + FileName;
            bool Result = false;
            mf.Dbase.DB.Close();
            try
            {
                if (File.Exists(Destination)) File.Delete(Destination);
                File.Copy(Source, Destination);
                Result = true;
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools:RestoreFile" + ex.Message);
            }
            mf.Dbase.OpenDatabase();
            return Result;
        }

        // http://www.leonardomiliani.com/en/2013/un-semplice-crc8-per-arduino/
        public byte CRC8(byte[] data, byte len)
        {
            byte crc = 0;
            byte sum = 0;
            byte extract = 0;
            for (int i = 0; i < len; i++)
            {
                extract = data[i];
                for (byte B = 8; B > 0; B--)
                {
                    sum = (byte)((crc ^ extract) & 0x01);
                    crc >>= 1;
                    if (sum > 0)
                    {
                        crc ^= 0x8C;
                    }
                    extract >>= 1;
                }
            }
            return crc;
        }

        public bool CRCmatch(byte[] Data, byte Len) 
        {
            // Len is total length of array, including CRC
            bool Result = false;
            if (Len > 0)
            {
                byte CRC = CRC8(Data, (byte)(Len - 1));
                Result = (CRC == Data[Len - 1]);
            }
            return Result;
        }
    }
}