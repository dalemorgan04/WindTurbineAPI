using WindTurbine.Domain.Entities;
using WindTurbineApi.Domain.Common;

namespace WindTurbineApi.Application.Interfaces
{
    public interface ISensorService
    {
        Task<Sensor?> GetSensorByIdAsync(Guid id);
        Task<Sensor?> GetSensorByNameAsync(string name);
        Task<IEnumerable<Sensor>> GetAllSensorsAsync();
        Task<Result<Sensor>> CreateSensorAsync(Sensor sensor);
        Task<Result> DeleteSensorAsync(Guid id);
    }
}
