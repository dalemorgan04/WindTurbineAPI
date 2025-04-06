namespace WindTurbineApi.Application.DTOs
{
    public class SensorDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public SensorDto(string name)
        {
            Name = name;
        }
    }
}
