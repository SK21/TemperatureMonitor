using System;
using System.Collections.Generic;

namespace TempMonitor
{
    public class clsPackets
    {
        //  PacketType:
        //  AllSensorsReport = 1,
        //  SingleSensor = 2,
        //  SetUserData = 3,

        private FormMain mf;
        private List<clsPacket> Packets = new List<clsPacket>();

        public clsPackets(FormMain CallingFrom)
        {
            mf = CallingFrom;
        }

        public void Add(PacketType Command, byte[] SensorAddress = null, int UserData = 0)
        {
            clsPacket NewPkt;
            DateTime StartTime = DateTime.Now;

            switch (Command)
            {
                case PacketType.AllSensorsReport:
                    // stagger control boxes
                    for (short ID = 0; ID < mf.MaxBoxes; ID++)
                    {
                        Packets.Add(new clsPacket { CommandID = Command, ControlBoxID = (byte)ID });
                        NewPkt = Packets[Packets.Count - 1]; // get the new packet
                        NewPkt.TimeStamp = StartTime.AddSeconds(mf.ReadSensorsDelay + (mf.ControlBoxDelay * ID));
                    }
                    break;

                case PacketType.SingleSensorReport:
                case PacketType.SetUserData:
                    // specific sensor report or set userdata
                    Packets.Add(new clsPacket { CommandID = Command });
                    NewPkt = Packets[Packets.Count - 1]; // get the new packet
                    NewPkt.ControlBoxID = ControlBoxID(SensorAddress);
                    NewPkt.SetAddressBytes(SensorAddress);
                    NewPkt.UserData = UserData;
                    NewPkt.TimeStamp = DateTime.Now;
                    break;
            }
        }

        public clsPacket Add(clsPacket NewPkt)
        {
            Packets.Add(new clsPacket { });
            clsPacket Pkt = Packets[Packets.Count - 1];

            Pkt.CommandID = NewPkt.CommandID;
            Pkt.ControlBoxID = NewPkt.ControlBoxID;
            Pkt.SetAddressBytes(NewPkt.GetAddressBytes());
            Pkt.UserData = NewPkt.UserData;
            Pkt.Temperature = NewPkt.Temperature;
            Pkt.TimeStamp = NewPkt.TimeStamp;
            return Pkt;
        }

        public int Count()
        {
            return Packets.Count;
        }

        public clsPacket Item(int ID)
        {
            return Packets[ID];
        }

        public bool Remove(int ID)
        {
            try
            {
                Packets.RemoveAt(ID);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SendNext()
        {
            // sends the next packet that has timed out
            if (Count() > 0)
            {
                for (int i = 0; i < Count(); i++)
                {
                    if (Item(i).TimeStamp <= DateTime.Now)
                    {
                        switch (Item(i).CommandID)
                        {
                            case PacketType.AllSensorsReport:
                                mf.SendSensorData.AllSensorsReport(Item(i).ControlBoxID);
                                break;

                            case PacketType.SingleSensorReport:
                                mf.SendSensorData.SpecificSensorReport(Item(i).ControlBoxID, Item(i).GetAddressBytes());
                                break;

                            case PacketType.SetUserData:
                                mf.SendSensorData.SetUserData(Item(i).ControlBoxID, Item(i).GetAddressBytes(), Item(i).UserData);
                                break;

                            default:
                                break;
                        }

                        Remove(i);
                        break;
                    }
                }
            }

            // check for too many outgoing packets
            while (((Count() > mf.MaxBoxes) & (mf.MaxBoxes > 0)) | (Count() > 50))
            {
                Remove(0);
            }
        }

        private byte ControlBoxID(byte[] SensorAddress)
        {
            clsSensor Sensor = new clsSensor(mf);
            Sensor.Load(SensorByteAddress: SensorAddress);
            return Sensor.ControlBoxID;
        }
    }
}