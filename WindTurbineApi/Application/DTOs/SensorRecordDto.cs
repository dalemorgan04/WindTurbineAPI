namespace WindTurbineApi.Application.DTOs
{
    public class SensorRecordDto
    {
        public Guid Id { get; set; }
        public Guid SensorId { get; set; }
        public required string SensorName { get; set; }
        public double Temperature { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
