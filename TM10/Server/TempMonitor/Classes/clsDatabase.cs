using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

// <Project><Add Reference...>, COM, Microsoft DAO 3.6 Object Library

namespace TempMonitor
{
    public class clsDatabase
    {
        public const byte cDBversion = 1;
        public const string DBtype = "TempMon";

        private readonly FormMain mf;

        private bool cConnected = false;
        private string cDatabaseName = "";
        private DAO.Database cDB;
        private DAO.DBEngine cDBE;

        public delegate void DBconnectedDelegate();
        public event DBconnectedDelegate DBconnected;

        public clsDatabase(FormMain CallingForm)
        {
            mf = CallingForm;
            cDBE = new DAO.DBEngine();
        }

        public bool Connected { get { return cConnected; } }

        public byte DBversion { get { return cDBversion; } }

        public string DBname(bool ShortName = false)
        {
            if (cConnected)
            {
                if (ShortName)
                {
                    return Path.GetFileNameWithoutExtension(cDB.Name);
                }
                else
                {
                    return cDB.Name;
                }
            }
            return "";
        }

        public string DBsize()
        {
            if (cConnected)
            {
                FileInfo FI = new FileInfo(cDB.Name);
                return (FI.Length / 1024).ToString("N0");
            }
            return "";
        }

        public string DBdate()
        {
            if (cConnected)
            {
                FileInfo FI = new FileInfo(cDB.Name);
                return FI.LastWriteTime.ToString();
            }
            return "";
        }

        public string DBfolder()
        {
            if (cConnected)
            {
                return Path.GetDirectoryName(cDB.Name);
            }
            return "";
        }

        public DAO.Database DB { get { return cDB; } }

        public DAO.DBEngine DBE { get { return cDBE; } }

        public bool OpenDatabase(string DBname = "", string DBpassword = "")
        {
            if (DBname == "")
            {
                // open last database
                DBname = mf.Tls.GetProperty("LastDatabase");
                DBpassword = mf.Tls.GetProperty("LastPassword");
            }
            if (File.Exists(DBname))
            {
                try
                {
                    try
                    {
                        // close previous database
                        cDatabaseName = "";
                        cConnected = false;
                        cDB.Close();
                    }
                    catch (Exception)
                    {
                    }
                    string DBvalid = CheckDatabase(DBname);
                    if (DBvalid == "true")
                    {
                        cDB = cDBE.OpenDatabase(DBname, DAO.DriverPromptEnum.dbDriverNoPrompt, false, ";pwd=" + DBpassword);
                        mf.Tls.SaveProperty("LastDatabase", DBname);
                        mf.Tls.SaveProperty("LastPassword", DBpassword);
                        cDatabaseName = Path.GetFileName(DBname);
                        cConnected = true;

                        // record database directory
                        mf.Tls.DataFolder = Path.GetDirectoryName(DBname);

                        // check database size
                        TrimRecords(cDB);

                        // raise database connected event
                        DBconnected?.Invoke();

                        return true;
                    }
                    else
                    {
                        mf.Tls.WriteErrorLog("clsDatabase: OpenDatabase: " + DBvalid);
                        mf.Tls.TimedMessageBox("Database not valid.", DBvalid);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    mf.Tls.WriteErrorLog("clsDatabase: OpenDatabase: " + ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string LastValue(string Table, string Field, string SortField = "", string Where = "")
        {
            // returns the last value of a field in a table
            DAO.Recordset RS;
            if (SortField == "") SortField = Field;
            string SQL = "Select * from " + Table;
            if (Where != "") SQL += " " + Where + " ";
            if (SortField != "") SQL += " order by " + SortField;

            RS = cDB.OpenRecordset(SQL);
            if (RS.EOF)
            {
                RS.Close();
                return "";
            }
            else
            {
                RS.MoveLast();
                if (RS.Fields[Field].Type == (short)DAO.DataTypeEnum.dbMemo |
                    RS.Fields[Field].Type == (short)DAO.DataTypeEnum.dbText)
                {
                    RS.Close();
                    return RS.Fields[Field].Value ?? "";
                }
                else
                {
                    RS.Close();
                    return RS.Fields[Field].Value ?? 0;
                }
            }
        }

        private string CheckDatabase(string DBname)
        {
            // check database type, version
            string Result = "";
            try
            {
                cDB = cDBE.OpenDatabase(DBname, DAO.DriverPromptEnum.dbDriverNoPrompt, false, "");
                DAO.Recordset RS;
                string SQL = "select * from tblProps";
                RS = cDB.OpenRecordset(SQL);
                if (RS.EOF)
                {
                    RS.Close();
                    cDB.Close();
                    Result = "No Data.";
                }
                else
                {
                    // check database type
                    string ReportedType = (string)(RS.Fields["dbType"].Value ?? "");
                    short ReportedVersion = (short)(RS.Fields["dbVersion"].Value ?? 0);
                    RS.Close();
                    cDB.Close();
                    if (ReportedType == DBtype)
                    {
                        // check version
                        Result = CheckVersion(ReportedVersion, DBname);
                    }
                    else
                    {
                        Result = "Wrong database type.";
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clsDatabase:CheckDatabase: " + ex.Message);
            }
            return Result;
        }

        private string CheckVersion(short Ver, string DBname)
        {
            string Result = "";
            if (Ver > cDBversion)
            {
                // higher version, program out of date
                Result = "Database version does not match, software out of date.";
            }
            else if (Ver == cDBversion)
            {
                Result = "true";
            }
            else
            {
                // lower version, update

                // close current database
                try
                {
                    cDB.Close();
                }
                catch (Exception)
                {
                }

                // copy base file to tmp file in data folder
                string NewPath = mf.Tls.DataFolder + "\\NewTmp.mdb";
                try
                {
                    // delete tmp file, if it exists
                    File.Delete(NewPath);
                }
                catch (Exception Ex)
                {
                    mf.Tls.WriteErrorLog("clsDatabase: CheckVersion: " + Ex.Message);
                }
                string BasePath = mf.Tls.SettingsFolder + "\\TempMonBase.mdb";
                FileInfo BaseFile = new FileInfo(BasePath);
                BaseFile.CopyTo(NewPath);

                // copy current database to tmp file in data folder
                string TmpPath = mf.Tls.DataFolder + "\\OldTmp.mdb";
                try
                {
                    // delete tmp file, if it exists
                    File.Delete(TmpPath);
                }
                catch (Exception Ex)
                {
                    mf.Tls.WriteErrorLog("clsDatabase: CheckVersion: " + Ex.Message);
                }
                FileInfo OldFile = new FileInfo(DBname);
                OldFile.CopyTo(TmpPath);

                // open new tmp database and old tmp database
                DAO.Database DBnew = cDBE.OpenDatabase(NewPath, DAO.DriverPromptEnum.dbDriverNoPrompt, false, "");
                DAO.Database DBold = cDBE.OpenDatabase(TmpPath, DAO.DriverPromptEnum.dbDriverNoPrompt, false, "");

                // copy matching data from old database to new database
                if (CopyData(DBnew, DBold))
                {
                    // update database properties
                    DAO.Recordset RS;
                    string SQL = "select * from tblProps";
                    RS = DBnew.OpenRecordset(SQL);
                    if (RS.EOF)
                    {
                        RS.AddNew();
                    }
                    else
                    {
                        RS.Edit();
                    }
                    RS.Fields["dbType"].Value = DBtype;
                    RS.Fields["dbVersion"].Value = cDBversion;
                    RS.Update();
                    RS.Close();

                    // copy new updated database to current database name
                    DBnew.Close();
                    DBold.Close();
                    File.Delete(DBname); // delete current file
                    FileInfo NewFile = new FileInfo(NewPath);
                    NewFile.CopyTo(DBname);  // copy new file to current file name
                    File.Delete(TmpPath);   // delete tmp copy of current file
                    File.Delete(NewPath);   // delete new file
                    Result = "true";
                }
                else
                {
                    // failed to copy, remove new database and tmp database
                    File.Delete(TmpPath);   // delete tmp copy of current file
                    File.Delete(NewPath);   // delete new file
                    Result = "Failed to update database version.";
                }
            }
            return Result;
        }

        private bool CopyData(DAO.Database DBnew, DAO.Database DBold)
        {
            DAO.Recordset RSnew;
            DAO.Recordset RSold;
            string TBL;
            List<string> Flds = new List<string>();
            try
            {
                foreach (DAO.TableDef TDnew in DBnew.TableDefs)
                {
                    TBL = TDnew.Name;
                    if (!IsSysTable(TBL))
                    {
                        Flds.Clear();   // erase list
                        // check if table exists in old database
                        foreach (DAO.TableDef TDold in DBold.TableDefs)
                        {
                            if (TDold.Name == TBL)
                            {
                                // make a list of compatable fields
                                foreach (DAO.Field FldNew in TDnew.Fields)
                                {
                                    if (!IsPrimaryKey(FldNew, TDnew))
                                    {
                                        foreach (DAO.Field Fldold in TDold.Fields)
                                        {
                                            if ((Fldold.Name == FldNew.Name) & (Fldold.Type == FldNew.Type))
                                            {
                                                // found matching field in old table
                                                Flds.Add(FldNew.Name);
                                                break;
                                            }
                                        }
                                    }
                                }

                                // copy data from matching fields
                                RSnew = DBnew.OpenRecordset(TBL);
                                RSold = DBold.OpenRecordset(TBL);
                                while (!RSold.EOF)
                                {
                                    RSnew.AddNew();
                                    foreach (string MatchingField in Flds)
                                    {
                                        RSnew.Fields[MatchingField].Value = RSold.Fields[MatchingField].Value;
                                    }
                                    RSnew.Update();
                                    RSold.MoveNext();
                                }
                                RSnew.Close();
                                RSold.Close();
                                break;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception Ex)
            {
                mf.Tls.WriteErrorLog("clsDatabase: CopyData: " + Ex.Message);
                return false;
            }
        }

        private bool IsSysTable(string TableName)
        {
            if (TableName.Length > 3)
            {
                if (TableName.Substring(0, 4).ToLower() == "msys") return true;
            }
            return false;
        }

        private bool IsPrimaryKey(DAO.Field Fld, DAO.TableDef Tbl)
        {
            // checks if fld is primary key
            foreach (DAO.Index IDX in Tbl.Indexes)
            {
                if (IDX.Primary)
                {
                    foreach (DAO.Field IDXfld in IDX.Fields)
                    {
                        if (IDXfld.Name == Fld.Name)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsAutoIncrField(DAO.Field Fld)
        {
            return (((int)(DAO.FieldAttributeEnum.dbAutoIncrField) & Fld.Attributes) == (int)(DAO.FieldAttributeEnum.dbAutoIncrField));
        }

        public void TrimRecords(DAO.Database DB)
        {
            DAO.Recordset RS;
            string SQL = "select * from tblProps";
            RS = DB.OpenRecordset(SQL);
            int dbMax = (int)(RS.Fields["dbMaxSize"].Value ?? 0);
            int Size = (int)(new System.IO.FileInfo(cDB.Name).Length);
            RS.Close();
            if (Size > dbMax)
            {
                SQL = "Select top 20 percent * from tblRecords order by recID desc";
                RS = DB.OpenRecordset(SQL);
                while (!RS.EOF)
                {
                    RS.Delete();
                    RS.MoveNext();
                }
                RS.Close();
            }
        }

        public bool CompactDatabase(string OldFileName)
        {
            bool Result = false;
            string TmpPath = mf.Tls.DataFolder + "\\OldTmp.mdb";
            try
            {
                try
                {
                    // delete tmp file, if it exists
                    File.Delete(TmpPath);
                }
                catch (Exception Ex)
                {
                    mf.Tls.WriteErrorLog("clsDatabase: CompactDatabase: " + Ex.Message);
                }
                FileInfo OldFile = new FileInfo(OldFileName);
                OldFile.CopyTo(TmpPath);
                DBE.Workspaces[0].Close();
                DBE.CompactDatabase(OldFileName, TmpPath);

                // check if new compacted file has been created
                if (File.Exists(TmpPath))
                {
                    // new file has been created, delete old file and rename new file
                    File.Delete(OldFileName);
                    FileInfo NewFile = new FileInfo(TmpPath);
                    NewFile.CopyTo(OldFileName);
                }
                Result = true;
            }
            catch (Exception)
            {
            }
            OpenDatabase(OldFileName);
            return Result;
        }

        public bool NewDatabase(string DBpath, bool CopyBins = false,
            bool CopySensors = false, bool CopyTemps = false, bool OpenNewDatabase = false)
        {
            bool Result = false;
            try
            {
                string DBname = Path.GetFileNameWithoutExtension(DBpath);
                if (LegalFileName(DBname))
                {
                    string NewPath = Path.GetDirectoryName(DBpath) + "\\" + DBname + ".mdb";
                    if (!File.Exists(NewPath))
                    {
                        // copy base file
                        //string BasePath = mf.Tls.SettingsFolder + "\\TempMonBase.mdb";
                        //FileInfo BaseFile = new FileInfo(BasePath);
                        //BaseFile.CopyTo(NewPath);

                        File.WriteAllBytes(NewPath, Properties.Resources.Base);

                        // check if previous database connected
                        if (cConnected)
                        {
                            // copy data
                            string SQL = "";
                            DAO.Recordset OldRS;
                            DAO.Recordset NewRS;
                            DAO.Database NewDB;
                            NewDB = cDBE.OpenDatabase(NewPath, DAO.DriverPromptEnum.dbDriverNoPrompt, false, "");

                            // database properties
                            SQL = "select * from tblProps";
                            NewRS = NewDB.OpenRecordset(SQL);
                            if (NewRS.EOF)
                            {
                                NewRS.AddNew();
                            }
                            else
                            {
                                NewRS.Edit();
                            }
                            OldRS = cDB.OpenRecordset(SQL);
                            NewRS.Fields["dbType"].Value = OldRS.Fields["dbType"].Value;
                            foreach (DAO.Field Fld in OldRS.Fields)
                            {
                                if (!IsAutoIncrField(Fld)) NewRS.Fields[Fld.Name].Value = OldRS.Fields[Fld.Name].Value;
                            }
                            NewRS.Update();
                            NewRS.Close();

                            // bins
                            if (CopyBins)
                            {
                                SQL = "select * from tblStorage";
                                OldRS = cDB.OpenRecordset(SQL);
                                NewRS = NewDB.OpenRecordset(SQL);
                                while (!OldRS.EOF)
                                {
                                    NewRS.AddNew();
                                    foreach (DAO.Field Fld in OldRS.Fields)
                                    {
                                        if (!IsAutoIncrField(Fld)) NewRS.Fields[Fld.Name].Value = OldRS.Fields[Fld.Name].Value;
                                    }
                                    NewRS.Update();
                                    OldRS.MoveNext();
                                }
                                NewRS.Close();
                                OldRS.Close();
                            }

                            // sensors
                            if (CopySensors)
                            {
                                SQL = "select * from tblSensors";
                                OldRS = cDB.OpenRecordset(SQL);
                                NewRS = NewDB.OpenRecordset(SQL);
                                while (!OldRS.EOF)
                                {
                                    NewRS.AddNew();
                                    foreach (DAO.Field Fld in OldRS.Fields)
                                    {
                                        if (!IsAutoIncrField(Fld)) NewRS.Fields[Fld.Name].Value = OldRS.Fields[Fld.Name].Value;
                                    }
                                    NewRS.Update();
                                    OldRS.MoveNext();
                                }
                                NewRS.Close();
                                OldRS.Close();
                            }

                            // temperature records
                            if (CopyTemps)
                            {
                                SQL = "select * from tblSensors";
                                OldRS = cDB.OpenRecordset(SQL);
                                NewRS = NewDB.OpenRecordset(SQL);
                                while (!OldRS.EOF)
                                {
                                    NewRS.AddNew();
                                    foreach (DAO.Field Fld in OldRS.Fields)
                                    {
                                        if (!IsAutoIncrField(Fld)) NewRS.Fields[Fld.Name].Value = OldRS.Fields[Fld.Name].Value;
                                    }
                                    NewRS.Update();
                                    OldRS.MoveNext();
                                }
                                NewRS.Close();
                                OldRS.Close();
                            }

                            NewDB.Close();
                        }
                        Result = true;
                        if (OpenNewDatabase) OpenDatabase(NewPath);
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clsDatabase:NewDatabase " + ex.Message);
                Result = false;
            }
            return Result;
        }

        public bool SaveAsDB(string Name, bool OpenNewDatabase = false)
        {
            bool Result = false;
            try
            {
                if (!File.Exists(Name))
                {
                    FileInfo OldDB = new FileInfo(cDB.Name);
                    try
                    {
                        cDB.Close();
                    }
                    catch (Exception)
                    {
                    }
                    OldDB.CopyTo(Name);
                    if (OpenNewDatabase) OpenDatabase(Name);
                    Result = true;
                }
            }
            catch (Exception)
            {
                return Result;
            }
            return Result; ;
        }

        public bool LegalFileName(string Name)
        {
            if (Name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string ToAccessDate(DateTime Date)
        {
            // date format for Access 97 database
            return "#" + Date.ToString("MM/dd/yyyy") + "#";
        }

        public void DeleteTable(string TBname)
        {
            try
            {
                mf.Dbase.DBE.BeginTrans();
                mf.Dbase.DB.Execute("Drop Table " + TBname);
                mf.Dbase.DBE.CommitTrans((int)DAO.CommitTransOptionsEnum.dbForceOSFlush);   // dbForceOSFlush = 1, DBE finishes before returning
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clsDatabase:DeleteTable " + ex.Message);
            }
        }

        public void EraseTable(string TBname)
        {
            try
            {
                string SQL = "Delete from " + TBname;
                mf.Dbase.DB.Execute(SQL);
            }
            catch (Exception ex)
            {

                mf.Tls.WriteErrorLog("clsDatabase:EraseTable " + ex.Message);
            }
        }

        public int FieldToInt(DAO.Recordset RS, string FieldName)
        {
            int Result = 0;
            try
            {
                if (!DBNull.Value.Equals(RS.Fields[FieldName].Value)) Result = (int)(RS.Fields[FieldName].Value);
            }
            catch (Exception)
            {


            }
            return Result;
        }

        public string FieldToString(DAO.Recordset RS, string FieldName)
        {
            string Result = "";
            try
            {
                if (!DBNull.Value.Equals(RS.Fields[FieldName].Value)) Result = (string)(RS.Fields[FieldName].Value);
            }
            catch (Exception)
            {

            }
            return Result;
        }

        public bool FieldToBool(DAO.Recordset RS, string FieldName)
        {
            bool Result = false;
            try
            {
                if (!DBNull.Value.Equals(RS.Fields[FieldName].Value)) Result = (bool)(RS.Fields[FieldName].Value);
            }
            catch (Exception)
            {

            }
            return Result;
        }

        public float FieldToFloat(DAO.Recordset RS, string FieldName)
        {
            float Result = 0;
            try
            {
                if (!DBNull.Value.Equals(RS.Fields[FieldName].Value)) Result = (float)(RS.Fields[FieldName].Value);
            }
            catch (Exception)
            {

            }
            return Result;
        }

        public byte ControlBoxCount()
        {
            byte Result = 0;
            try
            {
                DAO.Recordset RS;
                string SQL = "Select * from tblProps";
                RS = cDB.OpenRecordset(SQL);
                Result = (byte)FieldToInt(RS, "dbMaxBoxes");
                RS.Close();
            }
            catch (Exception)
            {

            }
            return Result;
        }


    }
}