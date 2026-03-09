using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinTempsApp.Models
{
    public class TemperatureRecord
    {
        [Key]
        public int Id { get; set; }

        public string RomCode { get; set; }
        public float Temperature { get; set; }
        public DateTime Timestamp { get; set; }

        [ForeignKey("RomCode")]
        public virtual Sensor Sensor { get; set; }
    }
}
