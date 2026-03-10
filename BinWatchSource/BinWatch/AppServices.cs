using System.Data.Entity;
using BinWatch.Data;
using BinWatch.Network;
using BinWatch.Services;

namespace BinWatch
{
    public static class AppServices
    {
        public static UdpServer UdpServer { get; private set; }
        public static PacketParser Parser { get; private set; }
        public static ModuleService ModuleService { get; private set; }
        public static TemperatureService TemperatureService { get; private set; }
        public static SensorService SensorService { get; private set; }

        public static void Initialize()
        {
            Database.SetInitializer<AppDbContext>(null);
            using (var db = new AppDbContext())
                db.EnsureSchema();

            UdpServer = new UdpServer();
            Parser = new PacketParser();
            ModuleService = new ModuleService(UdpServer);
            TemperatureService = new TemperatureService();
            SensorService = new SensorService(UdpServer);

            if (!AppConfig.PassiveMode)
            {
                // Wire events — skipped in passive mode (no UDP, DB is read-only view)
                UdpServer.PacketReceived += Parser.HandlePacket;
                Parser.ModuleDescriptionReceived += ModuleService.HandleModuleDescription;
                Parser.TemperatureReceived += TemperatureService.HandleTemperature;
                // TemperatureService.BatchRecorded wired by MainForm after construction
            }
        }

        public static void Start()
        {
            if (AppConfig.PassiveMode) return;

            UdpServer.Start();
            // Broadcast CmdSendModuleDescription so all online modules report back with
            // fresh 30831 packets (updates LastKnownIp even if DHCP assigned a new address)
            UdpServer.SendDiscovery();
        }

        public static void Shutdown()
        {
            UdpServer?.Stop();
            UdpServer?.Dispose();
        }
    }
}
