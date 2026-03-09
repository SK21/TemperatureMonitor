using System.Data.Entity;
using BinTempsApp.Data;
using BinTempsApp.Network;
using BinTempsApp.Services;

namespace BinTempsApp
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

            // Wire events
            UdpServer.PacketReceived += Parser.HandlePacket;
            Parser.ModuleDescriptionReceived += ModuleService.HandleModuleDescription;
            Parser.TemperatureReceived += TemperatureService.HandleTemperature;
        }

        public static void Start() => UdpServer.Start();

        public static void Shutdown()
        {
            UdpServer?.Stop();
            UdpServer?.Dispose();
        }
    }
}
