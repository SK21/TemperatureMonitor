using System.Data.Entity;
using System.Data.SQLite;
using BinTempsApp.Models;

namespace BinTempsApp.Data
{
    public class AppDbContext : DbContext
    {
        public static string DbPath => AppConfig.ResolvedDbPath;

        public AppDbContext()
            : base(new SQLiteConnection($"Data Source={DbPath};BusyTimeout=5000"), contextOwnsConnection: true)
        {
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<TemperatureRecord> Records { get; set; }
        public DbSet<Setting> Settings { get; set; }

        /// <summary>
        /// Creates all tables if they do not already exist. Safe to call on every startup.
        /// Uses raw SQL because EF6's Database.Create() is unreliable with SQLite.
        /// </summary>
        public void EnsureSchema()
        {
            // WAL mode allows concurrent readers + one writer without "database is locked"
            // errors when the background UDP thread writes while the UI thread reads.
            // This setting is persistent — only needs to be applied once per database file.
            Database.ExecuteSqlCommand(
                System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction,
                "PRAGMA journal_mode=WAL;");

            Database.ExecuteSqlCommand(@"
                CREATE TABLE IF NOT EXISTS Modules (
                    MacAddress      TEXT    PRIMARY KEY NOT NULL,
                    ModuleId        INTEGER NOT NULL DEFAULT 0,
                    Name            TEXT,
                    LastKnownIp     TEXT,
                    LastSeen        TEXT,
                    Status          TEXT,
                    FirmwareVersion INTEGER NOT NULL DEFAULT 0
                )");

            Database.ExecuteSqlCommand(@"
                CREATE TABLE IF NOT EXISTS Sensors (
                    RomCode   TEXT    PRIMARY KEY NOT NULL,
                    ModuleMac TEXT,
                    BinId     INTEGER NOT NULL DEFAULT 0,
                    CableId   INTEGER NOT NULL DEFAULT 0,
                    SensorNum INTEGER NOT NULL DEFAULT 0,
                    Enabled   INTEGER NOT NULL DEFAULT 1,
                    Offset    REAL    NOT NULL DEFAULT 0.0,
                    MaxTemp   REAL    NOT NULL DEFAULT 40.0,
                    Label     TEXT,
                    FOREIGN KEY (ModuleMac) REFERENCES Modules (MacAddress)
                )");

            Database.ExecuteSqlCommand(@"
                CREATE TABLE IF NOT EXISTS TemperatureRecords (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    RomCode     TEXT,
                    Temperature REAL    NOT NULL DEFAULT 0.0,
                    Timestamp   TEXT    NOT NULL,
                    FOREIGN KEY (RomCode) REFERENCES Sensors (RomCode)
                )");

            Database.ExecuteSqlCommand(@"
                CREATE TABLE IF NOT EXISTS Settings (
                    Key   TEXT PRIMARY KEY NOT NULL,
                    Value TEXT
                )");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sensor>()
                .HasOptional(s => s.Module)
                .WithMany(m => m.Sensors)
                .HasForeignKey(s => s.ModuleMac);

            modelBuilder.Entity<TemperatureRecord>()
                .HasOptional(r => r.Sensor)
                .WithMany(s => s.Records)
                .HasForeignKey(r => r.RomCode);
        }
    }
}
