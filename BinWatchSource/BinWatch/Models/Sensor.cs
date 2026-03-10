using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinWatch.Models
{
    public class Sensor
    {
        [Key]
        public string RomCode { get; set; }     // hex string "AABBCCDDEEFF0011"

        public string ModuleMac { get; set; }
        public byte BinId { get; set; }
        public byte CableId { get; set; }
        public byte SensorNum { get; set; }
        public bool Enabled { get; set; } = true;
        public float Offset { get; set; } = 0.0f;
        public float MaxTemp { get; set; } = 40.0f;
        public string Label { get; set; }

        /// <summary>Raw 16-bit user data from the last received packet. Null until first packet.</summary>
        public int? RawUserData { get; set; }

        /// <summary>
        /// When true, incoming temperature packets will not overwrite BinId/CableId/SensorNum.
        /// Set automatically when a sensor is converted via ConvertSensorsForm.
        /// Clear it (via SensorEditForm) to allow the sensor to resume self-reporting its location.
        /// </summary>
        public bool ManualLocation { get; set; } = false;

        public int? FormatId { get; set; }

        [ForeignKey("FormatId")]
        public virtual UserDataFormatDef Format { get; set; }

        [ForeignKey("ModuleMac")]
        public virtual Module Module { get; set; }

        public virtual ICollection<TemperatureRecord> Records { get; set; } = new List<TemperatureRecord>();
    }
}
