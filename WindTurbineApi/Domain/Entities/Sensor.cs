using System.ComponentModel.DataAnnotations;

namespace WindTurbine.Domain.Entities
{
    // Represents a temperature sensor on the wind turbine
    public class Sensor
    {
        // Unique identifier for the temperature sensor
        public required Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public ICollection<SensorRecord>? SensorRecords { get; set; }
    }
}
