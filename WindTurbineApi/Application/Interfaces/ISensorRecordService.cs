using WindTurbine.Domain.Entities;
using WindTurbineApi.Domain.Common;

namespace WindTurbineApi.Application.Interfaces
{
    public interface ISensorRecordService
    {
        Task<SensorRecord?> GetSensorRecordByIdAsync(Guid id);

        Task<IEnumerable<SensorRecord>> GetFilteredAsync(
            string? sensorName = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            double? aboveValue = null,
            double? belowValue = null);
        Task<SensorRecord?> CreateSensorRecordAsync(SensorRecord record);
        Task<Result> DeleteSensorRecordAsync(Guid id);
    }
}