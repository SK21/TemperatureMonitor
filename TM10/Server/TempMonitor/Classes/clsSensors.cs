using System;
using System.Collections.Generic;

namespace TempMonitor
{
    public class clsSensors
    {
        public IList<clsSensor> Items;
        private List<clsSensor> cSensors = new List<clsSensor>();
        private FormMain mf;

        public clsSensors(FormMain CallingForm)
        {
            mf = CallingForm;
            Items = cSensors.AsReadOnly();  // to use ForEach
        }

        public clsSensor Add(byte[] Address)
        {
            // check if exists
            short ID = ByteAddressFound(Address);
            if (ID == -1)
            {
                // new address
                cSensors.Add(new clsSensor(mf));
                clsSensor Sen = cSensors[cSensors.Count - 1];
                Sen.SetAddressBytes(Address);
                return Sen;
            }
            else
            {
                // existing address
                clsSensor Sen = new clsSensor(mf);
                Sen.Load(ID);
                return Sen;
            }
        }

        public clsSensor Add(clsSensor NewSensor)
        {
            if (NewSensor.ValidationErrors() != "") throw new ArgumentException(NewSensor.ValidationErrors());

            cSensors.Add(new clsSensor(mf));
            clsSensor Sen = cSensors[cSensors.Count - 1];
            Sen.SensorAddress = NewSensor.SensorAddress;
            Sen.ControlBoxID = NewSensor.ControlBoxID;
            Sen.Enabled = NewSensor.Enabled;
            Sen.OffSet = NewSensor.OffSet;
            Sen.UserData = NewSensor.UserData;
            return Sen;
        }

        public short AddressFound(string Adr)
        {
            short result = -1;
            foreach (clsSensor Sen in cSensors)
            {
                if (Sen.SensorAddress == Adr)
                {
                    result = Sen.ID;
                    break;
                }
            }
            return result;
        }

        public short ByteAddressFound(byte[] AddressBytes)
        {
            // returns sensor ID if found

            short Result = -1;
            if (AddressBytes.Length == 8)
            {
                string Adr = BitConverter.ToString(AddressBytes).Replace("-", " ");
                Result = AddressFound(Adr);
            }
            return Result;
        }

        public List<byte> ControlBoxIDs(bool OnlyEnabledSensors = false)
        {
            List<byte> IDs = new List<byte>();
            DAO.Recordset RS;
            string SQL = "select senControlBoxID, Max(recTimeStamp) as TS";
            SQL += " From tblSensors Left Join tblRecords On tblSensors.senID = tblRecords.recSenID";
            SQL += " Group By senControlBoxID";
            if (OnlyEnabledSensors) SQL += ", senEnabled having senEnabled = True";
            SQL += " Order By Max(recTimeStamp) Desc";

            RS = mf.Dbase.DB.OpenRecordset(SQL);
            while (!RS.EOF)
            {
                IDs.Add((byte)(RS.Fields["senControlBoxID"].Value ?? 0));
                RS.MoveNext();
            }
            RS.Close();
            return IDs;
        }

        public int Count()
        {
            return cSensors.Count;
        }

        public void Delete(int SensorID)
        {
            int IDX = ListID(SensorID);
            if (IDX != -1)
            {
                // remove from list
                cSensors.RemoveAt(IDX);

                // remove from database
                DAO.Recordset RS;
                string SQL = "Select * from tblSensors where senID = " + SensorID.ToString();
                RS = mf.Dbase.DB.OpenRecordset(SQL);
                if (!RS.EOF) RS.Delete();
                RS.Close();
            }
        }

        public clsSensor Item(int SensorID)
        {
            int IDX = ListID(SensorID);
            if (IDX == -1) throw new IndexOutOfRangeException();
            return cSensors[IDX];
        }

        public void Load()
        {
            cSensors.Clear();

            DAO.Recordset RS;
            string SQL = "Select * from tblSensors order by senRecNum";
            RS = mf.Dbase.DB.OpenRecordset(SQL);
            while (!RS.EOF)
            {
                clsSensor Sen = new clsSensor(mf);
                Sen.Load((short)RS.Fields["senID"].Value);
                cSensors.Add(Sen);
                RS.MoveNext();
            }
            RS.Close();
        }

        private int ListID(int SensorID)
        {
            for (int i = 0; i < cSensors.Count; i++)
            {
                if (cSensors[i].ID == SensorID) return i;
            }
            return -1;
        }
    }
}