using System;
using System.Linq;
using BinTempsApp.Data;
using BinTempsApp.Models;
using BinTempsApp.Network;

namespace BinTempsApp.Services
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
                module.LastSeen = DateTime.Now;
                module.Status = packet.IsUnregistered ? "Unregistered" : "Online";
                module.FirmwareVersion = packet.FirmwareVersion;

                db.SaveChanges();

                ModuleUpdated?.Invoke(this, new ModuleUpdatedEventArgs(module));
            }
        }

        // Assign an ID and name to an unregistered module — sends PGN 30822
        public void RegisterModule(byte[] mac, byte newId, string name)
        {
            _udpServer.SendSetModuleDescription(mac, newId, name);
        }

        public void SetModuleOffline(string macAddress)
        {
            using (var db = new AppDbContext())
            {
                var module = db.Modules.Find(macAddress);
                if (module == null) return;
                module.Status = "Offline";
                db.SaveChanges();

                ModuleUpdated?.Invoke(this, new ModuleUpdatedEventArgs(module));
            }
        }

        public Module[] GetAllModules()
        {
            using (var db = new AppDbContext())
                return db.Modules.ToArray();
        }
    }
}
