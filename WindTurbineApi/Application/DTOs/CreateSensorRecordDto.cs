namespace WindTurbineApi.Application.DTOs
{
    public class CreateSensorRecordDto
    {
        public string SensorName { get; set; }
        public string Timestamp { get; set; }

        public double Temperature { get; set; }

        public CreateSensorRecordDto(string sensorName, string timeStamp, double temperature )
        {
            SensorName = sensorName;
            Timestamp = timeStamp;
            Temperature = temperature;
        }
    }
}