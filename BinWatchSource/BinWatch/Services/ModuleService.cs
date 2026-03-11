using System;
using System.Linq;
using BinWatch.Data;
using BinWatch.Models;
using BinWatch.Network;

namespace BinWatch.Services
{
    public class ModuleUpdatedEventArgs : EventArgs
    {
        public Module Module { get; }
        public ModuleUpdatedEventArgs(Module module) { Module = module; }
    }

    public class ModuleService
    {
        private readonly UdpServer _udpServer;

        public event EventHandler<ModuleUpdatedEventArgs> ModuleUpdated;

        public ModuleService(UdpServer udpServer)
        {
            _udpServer = udpServer;
        }

        // Subscribe this to PacketParser.ModuleDescriptionReceived
        public void HandleModuleDescription(object sender, ModuleDescriptionPacket packet)
        {
            DateTime lastSeen = DateTime.Now;
            using (var db = new AppDbContext())
            {
                string mac = packet.MacString;
                var module = db.Modules.Find(mac);

                if (module == null)
                {
                    module = new Module { MacAddress = mac };
                    db.Modules.Add(module);
                }

                module.ModuleId = packet.ModuleId;
                module.Name = packet.Name;
                module.LastKnownIp = packet.Source.Address.ToString();
                module.LastSeen = lastSeen;
                module.FirmwareVersion = packet.FirmwareVersion;

                db.SaveChanges();
            }

            // Pass a plain POCO snapshot — avoids EF6 proxy interference on detached entities.
            var snapshot = new Module
            {
                MacAddress  = packet.MacString,
                ModuleId    = packet.ModuleId,
                Name        = packet.Name,
                LastKnownIp = packet.Source.Address.ToString(),
                LastSeen    = lastSeen,
                FirmwareVersion = packet.FirmwareVersion
            };
            ModuleUpdated?.Invoke(this, new ModuleUpdatedEventArgs(snapshot));
        }

        // Assign an ID and name to an unregistered module — sends PGN 30822
        public void RegisterModule(byte[] mac, byte newId, string name, string targetIp = null)
        {
            _udpServer.SendSetModuleDescription(mac, newId, name, targetIp);
        }

        public void RaiseModuleUpdated(Module module)
        {
            ModuleUpdated?.Invoke(this, new ModuleUpdatedEventArgs(module));
        }

        public event EventHandler<ModuleUpdatedEventArgs> ModuleLastSeenUpdated;

        public void RaiseLastSeenUpdated(Module module)
        {
            ModuleLastSeenUpdated?.Invoke(this, new ModuleUpdatedEventArgs(module));
        }

        public Module[] GetAllModules()
        {
            using (var db = new AppDbContext())
                return db.Modules.ToArray();
        }

        // Save name/ID locally and fire ModuleUpdated (also send UDP if module is reachable)
        public void SaveModuleLocally(string macAddress, byte newId, string name)
        {
            using (var db = new AppDbContext())
            {
                var module = db.Modules.Find(macAddress);
                if (module == null) return;
                module.ModuleId = newId;
                module.Name = name;
                db.SaveChanges();
                ModuleUpdated?.Invoke(this, new ModuleUpdatedEventArgs(module));
            }
        }
    }
}
