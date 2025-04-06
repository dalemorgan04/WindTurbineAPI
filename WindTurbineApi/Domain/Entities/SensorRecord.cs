using System.ComponentModel.DataAnnotations;
using WindTurbineApi.Domain.ValueObjects;

namespace WindTurbine.Domain.Entities
{
    public class SensorRecord
    {
        public Guid Id { get; set; }

        public Guid SensorId { get; set; }
        [Required]
        public required Sensor Sensor { get; set; }
        [Required]
        public required DateTime Timestamp { get; set; }
        // EF converts double to a Real in the db which is a floating point number as required
        [Required]
        public required ReadingValue Reading { get; set; }
    }
}
