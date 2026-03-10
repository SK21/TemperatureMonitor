using System.Collections.Generic;
using System.Linq;
using BinWatch.Data;
using BinWatch.Models;
using BinWatch.Network;

namespace BinWatch.Services
{
    public class SensorService
    {
        private readonly UdpServer _udpServer;

        public SensorService(UdpServer udpServer)
        {
            _udpServer = udpServer;
        }

        public List<Sensor> GetAll()
        {
            using (var db = new AppDbContext())
                return db.Sensors.Include("Module").ToList();
        }

        public Sensor GetByRomCode(string romCode)
        {
            using (var db = new AppDbContext())
                return db.Sensors.Include("Module").FirstOrDefault(s => s.RomCode == romCode);
        }

        public List<Sensor> GetByModule(string macAddress)
        {
            using (var db = new AppDbContext())
                return db.Sensors.Where(s => s.ModuleMac == macAddress).ToList();
        }

        public void UpdateLabel(string romCode, string label)
        {
            using (var db = new AppDbContext())
            {
                var sensor = db.Sensors.Find(romCode);
                if (sensor == null) return;
                sensor.Label = label;
                db.SaveChanges();
            }
        }

        public void UpdateOffset(string romCode, float offset)
        {
            using (var db = new AppDbContext())
            {
                var sensor = db.Sensors.Find(romCode);
                if (sensor == null) return;
                sensor.Offset = offset;
                db.SaveChanges();
            }
        }

        public void UpdateMaxTemp(string romCode, float maxTemp)
        {
            using (var db = new AppDbContext())
            {
                var sensor = db.Sensors.Find(romCode);
                if (sensor == null) return;
                sensor.MaxTemp = maxTemp;
                db.SaveChanges();
            }
        }

        public void SetEnabled(string romCode, bool enabled)
        {
            using (var db = new AppDbContext())
            {
                var sensor = db.Sensors.Find(romCode);
                if (sensor == null) return;
                sensor.Enabled = enabled;
                db.SaveChanges();
            }
        }

        public void UpdateLocation(string romCode, byte binId, byte cableId, byte sensorNum)
        {
            using (var db = new AppDbContext())
            {
                var sensor = db.Sensors.Find(romCode);
                if (sensor == null) return;
                sensor.BinId     = binId;
                sensor.CableId   = cableId;
                sensor.SensorNum = sensorNum;
                db.SaveChanges();
            }
        }

        // Sends PGN 30823 to write updated user data to the sensor's EEPROM
        public void SendUserData(byte moduleId, byte[] romCodeBytes, byte binId,
            byte cableId, byte sensorNum)
        {
            // Encode: [15:8] Bin | [7:4] Cable | [3:0] Sensor
            ushort raw = (ushort)((binId << 8) | (cableId << 4) | sensorNum);
            byte ud0 = (byte)(raw & 0xFF);
            byte ud1 = (byte)(raw >> 8);
            _udpServer.SendSetSensorUserData(moduleId, romCodeBytes, ud0, ud1);
        }
    }
}
