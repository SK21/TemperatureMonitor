using System;
using System.Collections.Generic;
using System.Linq;
using BinWatch.Data;
using BinWatch.Models;
using BinWatch.Network;

namespace BinWatch.Services
{
    public class TemperatureRecordedEventArgs : EventArgs
    {
        public TemperatureRecord Record { get; }
        public TemperatureRecordedEventArgs(TemperatureRecord record) { Record = record; }
    }

    public class TemperatureService
    {
        public event EventHandler<TemperatureRecordedEventArgs> TemperatureRecorded;

        // Subscribe this to PacketParser.TemperatureReceived
        public void HandleTemperature(object sender, TemperaturePacket packet)
        {
            using (var db = new AppDbContext())
            {
                string romCode = packet.RomCodeHex;

                // Resolve module MAC from the packet's module ID
                string moduleMac = null;
                if (packet.ModuleId != 0)
                    moduleMac = db.Modules.Where(m => m.ModuleId == packet.ModuleId)
                                          .Select(m => m.MacAddress)
                                          .FirstOrDefault();

                // Ensure sensor exists
                var sensor = db.Sensors.Find(romCode);
                if (sensor == null)
                {
                    sensor = new Sensor
                    {
                        RomCode = romCode,
                        ModuleMac = moduleMac,
                        BinId = packet.BinId,
                        CableId = packet.CableId,
                        SensorNum = packet.SensorNum,
                        Enabled = true
                    };
                    db.Sensors.Add(sensor);
                }
                else
                {
                    // Update position from user data if it has changed
                    sensor.BinId = packet.BinId;
                    sensor.CableId = packet.CableId;
                    sensor.SensorNum = packet.SensorNum;
                    if (moduleMac != null && sensor.ModuleMac == null)
                        sensor.ModuleMac = moduleMac;
                }

                if (!sensor.Enabled) return;

                float temperature = packet.Temperature + sensor.Offset;

                var record = new TemperatureRecord
                {
                    RomCode = romCode,
                    Temperature = temperature,
                    Timestamp = DateTime.Now
                };

                db.Records.Add(record);
                db.SaveChanges();

                TemperatureRecorded?.Invoke(this, new TemperatureRecordedEventArgs(record));
            }
        }

        public List<TemperatureRecord> GetLatestPerSensor()
        {
            using (var db = new AppDbContext())
            {
                // LINQ GroupBy+FirstOrDefault generates OUTER APPLY which SQLite doesn't support.
                // Use a correlated subquery instead.
                return db.Database.SqlQuery<TemperatureRecord>(
                    @"SELECT * FROM TemperatureRecords t
                      WHERE t.Timestamp = (
                          SELECT MAX(t2.Timestamp) FROM TemperatureRecords t2
                          WHERE t2.RomCode = t.RomCode
                      )"
                ).ToList();
            }
        }

        public List<TemperatureRecord> GetAllHistory(DateTime from, DateTime to)
        {
            using (var db = new AppDbContext())
            {
                return db.Records
                    .Where(r => r.Timestamp >= from && r.Timestamp <= to)
                    .OrderBy(r => r.RomCode).ThenBy(r => r.Timestamp)
                    .ToList();
            }
        }

        public List<TemperatureRecord> GetHistory(string romCode, DateTime from, DateTime to)
        {
            using (var db = new AppDbContext())
            {
                return db.Records
                    .Where(r => r.RomCode == romCode && r.Timestamp >= from && r.Timestamp <= to)
                    .OrderBy(r => r.Timestamp)
                    .ToList();
            }
        }
    }
}
