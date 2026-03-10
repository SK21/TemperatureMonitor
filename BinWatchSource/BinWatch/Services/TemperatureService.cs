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

    public class TemperatureBatchEventArgs : EventArgs
    {
        public byte ModuleId { get; }
        public string ModuleName { get; }
        public IReadOnlyList<(Sensor Sensor, TemperatureRecord Record)> Entries { get; }

        public TemperatureBatchEventArgs(byte moduleId, string moduleName,
            IReadOnlyList<(Sensor, TemperatureRecord)> entries)
        {
            ModuleId = moduleId;
            ModuleName = moduleName;
            Entries = entries;
        }
    }

    public class TemperatureService
    {
        public event EventHandler<TemperatureBatchEventArgs> BatchRecorded;

        private readonly object _batchLock = new object();
        private readonly Dictionary<byte, List<TemperaturePacket>> _batches =
            new Dictionary<byte, List<TemperaturePacket>>();

        // Subscribe this to PacketParser.TemperatureReceived
        public void HandleTemperature(object sender, TemperaturePacket packet)
        {
            List<TemperaturePacket> batch = null;

            lock (_batchLock)
            {
                if (!_batches.ContainsKey(packet.ModuleId))
                    _batches[packet.ModuleId] = new List<TemperaturePacket>();

                _batches[packet.ModuleId].Add(packet);

                if (packet.SensorsRemaining == 0)
                {
                    batch = _batches[packet.ModuleId];
                    _batches.Remove(packet.ModuleId);
                }
            }

            if (batch != null)
                FlushBatch(batch);
        }

        private void FlushBatch(List<TemperaturePacket> packets)
        {
            if (packets.Count == 0) return;

            byte moduleId = packets[0].ModuleId;
            string moduleName = null;
            var entries = new List<(Sensor, TemperatureRecord)>();

            try
            {
                using (var db = new AppDbContext())
                {
                    // Resolve module MAC once for the whole batch
                    string moduleMac = null;
                    if (moduleId != 0)
                    {
                        var module = db.Modules.FirstOrDefault(m => m.ModuleId == moduleId);
                        moduleMac = module?.MacAddress;
                        moduleName = module?.Name;
                    }

                    foreach (var packet in packets)
                    {
                        string romCode = packet.RomCodeHex;
                        var sensor = db.Sensors.Find(romCode);

                        if (sensor == null)
                        {
                            sensor = new Sensor
                            {
                                RomCode     = romCode,
                                ModuleMac   = moduleMac,
                                BinId       = packet.BinId,
                                CableId     = packet.CableId,
                                SensorNum   = packet.SensorNum,
                                RawUserData = (packet.UserData1 << 8) | packet.UserData0,
                                Enabled     = true
                            };
                            db.Sensors.Add(sensor);
                        }
                        else
                        {
                            if (!sensor.ManualLocation)
                            {
                                sensor.BinId    = packet.BinId;
                                sensor.CableId  = packet.CableId;
                                sensor.SensorNum = packet.SensorNum;
                            }
                            if (moduleMac != null && sensor.ModuleMac == null)
                                sensor.ModuleMac = moduleMac;
                        }

                        // Always capture raw user data so ConvertSensorsForm can re-decode later
                        sensor.RawUserData = (packet.UserData1 << 8) | packet.UserData0;

                        if (!sensor.Enabled) continue;

                        float temperature = packet.Temperature + sensor.Offset;
                        var record = new TemperatureRecord
                        {
                            RomCode     = romCode,
                            Temperature = temperature,
                            Timestamp   = DateTime.Now
                        };
                        db.Records.Add(record);
                        entries.Add((sensor, record));
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to flush temperature batch from module {moduleId}", ex);
                return;
            }

            if (entries.Count > 0)
                BatchRecorded?.Invoke(this, new TemperatureBatchEventArgs(moduleId, moduleName, entries));
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
