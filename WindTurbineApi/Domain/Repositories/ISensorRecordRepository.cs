using WindTurbine.Domain.Entities;

namespace WindTurbineApi.Domain.Repositories
{
    public interface ISensorRecordRepository
    {
        Task<SensorRecord?> GetByIdAsync(Guid id);
        Task<SensorRecord?> GetByNameAsync(string name);
        Task<IEnumerable<SensorRecord>> GetAllAsync();
        Task<IEnumerable<SensorRecord>> GetFilteredAsync(
            string? sensorName,
            DateTime? startDate,
            DateTime? endDate,
            double? aboveValue,
            double? belowValue);

        Task<IEnumerable<SensorRecord>> GetAllByDateTimeRangeAsync(DateTime fromDateTime, DateTime toDateTime);
        Task<IEnumerable<SensorRecord>> GetAllByTemperatureRangeAsync(double fromTemperature, double toTemperature);
        Task AddAsync(SensorRecord sensor);
        void UpdateAsync(SensorRecord sensor);
        Task DeleteAsync(Guid id);
    }
}
