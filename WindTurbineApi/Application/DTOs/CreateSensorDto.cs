namespace WindTurbineApi.Application.DTOs
{
    public class CreateSensorDto
    {
        public string Name { get; set; }
        public CreateSensorDto(string name)
        {
            Name = name;
        }
    }
}
