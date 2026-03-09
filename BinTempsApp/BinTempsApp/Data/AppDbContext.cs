using System.Data.Entity;
using System.IO;
using BinTempsApp.Models;

namespace BinTempsApp.Data
{
    public class AppDbContext : DbContext
    {
        public static string DbPath =>
            Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "BinTemps.db");

        public AppDbContext()
            : base($"Data Source={DbPath}")
        {
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<TemperatureRecord> Records { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Sensor -> Module (MacAddress FK)
            modelBuilder.Entity<Sensor>()
                .HasOptional(s => s.Module)
                .WithMany(m => m.Sensors)
                .HasForeignKey(s => s.ModuleMac);

            // TemperatureRecord -> Sensor (RomCode FK)
            modelBuilder.Entity<TemperatureRecord>()
                .HasOptional(r => r.Sensor)
                .WithMany(s => s.Records)
                .HasForeignKey(r => r.RomCode);
        }
    }
}
