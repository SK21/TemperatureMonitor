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

        private readonly ModuleService _moduleService;
        private readonly object _batchLock = new object();
        private readonly Dictionary<byte, List<TemperaturePacket>> _batches =
            new Dictionary<byte, List<TemperaturePacket>>();
        private readonly Dictionary<byte, DateTime> _batchLastPacket =
            new Dictionary<byte, DateTime>();

        // Flush incomplete batches after this long with no new packets (weak WiFi recovery)
        private static readonly TimeSpan BatchTimeout = TimeSpan.FromSeconds(15);
        private readonly System.Threading.Timer _timeoutTimer;

        public TemperatureService(ModuleService moduleService)
        {
            _moduleService = moduleService;
            _timeoutTimer = new System.Threading.Timer(_ => FlushTimedOutBatches(), null,
                TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15));
        }

        // Subscribe this to PacketParser.TemperatureReceived
        public void HandleTemperature(object sender, TemperaturePacket packet)
        {
            List<TemperaturePacket> batch = null;

            lock (_batchLock)
            {
                if (!_batches.ContainsKey(packet.ModuleId))
                    _batches[packet.ModuleId] = new List<TemperaturePacket>();

                var existing = _batches[packet.ModuleId];

                // In a valid batch, (SensorsRemaining + packetCount) is constant.
                // If it increases, the previous batch lost its last packet — discard it.
                if (existing.Count > 0 &&
                    (packet.SensorsRemaining + existing.Count) > existing[0].SensorsRemaining)
                {
                    Logger.Warning($"Discarding stale batch for module {packet.ModuleId} ({existing.Count} packets)");
                    existing.Clear();
                }

                existing.Add(packet);
                _batchLastPacket[packet.ModuleId] = DateTime.UtcNow;

                if (packet.SensorsRemaining == 0)
                {
                    batch = existing;
                    _batches.Remove(packet.ModuleId);
                    _batchLastPacket.Remove(packet.ModuleId);
                }
            }

            if (batch != null)
                FlushBatch(batch);
        }

        private void FlushTimedOutBatches()
        {
            var toFlush = new List<List<TemperaturePacket>>();
            lock (_batchLock)
            {
                var now = DateTime.UtcNow;
                foreach (var kvp in _batchLastPacket)
                {
                    if ((now - kvp.Value) < BatchTimeout) continue;
                    byte moduleId = kvp.Key;
                    if (!_batches.TryGetValue(moduleId, out var batch)) continue;
                    Logger.Warning($"Flushing incomplete batch for module {moduleId} ({batch.Count} of {batch[0].SensorsRemaining + 1} packets) after timeout");
                    toFlush.Add(batch);
                    _batches.Remove(moduleId);
                }
                foreach (var batch in toFlush)
                    _batchLastPacket.Remove(batch[0].ModuleId);
            }
            foreach (var batch in toFlush)
                FlushBatch(batch);
        }

        private void FlushBatch(List<TemperaturePacket> packets)
        {
            if (packets.Count == 0) return;

            byte moduleId = packets[0].ModuleId;
            string moduleName = null;
            var entries = new List<(Sensor, TemperatureRecord)>();
            Module touchedModule = null;

            try
            {
                using (var db = new AppDbContext())
                {
                    // Resolve module MAC once for the whole batch
                    string moduleMac = null;
                    if (moduleId != 0)
                    {
                        var module = db.Modules.FirstOrDefault(m => m.ModuleId == moduleId);
                        if (module != null)
                        {
                            moduleMac = module.MacAddress;
                            moduleName = module.Name;
                            module.LastSeen = DateTime.Now;
                            touchedModule = module;
                        }
                    }

                    var formats = db.Formats.ToList().ToDictionary(f => f.Id);

                    foreach (var packet in packets)
                    {
                        string romCode = packet.RomCodeHex;
                        var sensor = db.Sensors.Find(romCode);
                        ushort rawUserData = (ushort)((packet.UserData1 << 8) | packet.UserData0);

                        if (sensor == null)
                        {
                            sensor = new Sensor
                            {
                                RomCode     = romCode,
                                ModuleMac   = moduleMac,
                                BinId       = packet.BinId,
                                CableId     = packet.CableId,
                                SensorNum   = packet.SensorNum,
                                RawUserData = rawUserData,
                                Enabled     = true
                            };
                            db.Sensors.Add(sensor);
                        }
                        else
                        {
                            sensor.RawUserData = rawUserData;

                            if (sensor.FormatId.HasValue &&
                                formats.TryGetValue(sensor.FormatId.Value, out var fmt))
                            {
                                var (bin, cable, sensorNum) = fmt.Decode(rawUserData);
                                sensor.BinId    = (byte)bin;
                                sensor.CableId  = (byte)cable;
                                sensor.SensorNum = (byte)sensorNum;
                            }
                            else if (!sensor.ManualLocation)
                            {
                                sensor.BinId    = packet.BinId;
                                sensor.CableId  = packet.CableId;
                                sensor.SensorNum = packet.SensorNum;
                            }

                            if (moduleMac != null && sensor.ModuleMac == null)
                                sensor.ModuleMac = moduleMac;
                        }

                        // Always capture raw user data
                        sensor.RawUserData = rawUserData;

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

            if (touchedModule != null)
                _moduleService.RaiseLastSeenUpdated(touchedModule);

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
