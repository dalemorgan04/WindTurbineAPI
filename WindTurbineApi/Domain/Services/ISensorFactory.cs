using WindTurbine.Domain.Entities;
using WindTurbineApi.Domain.Common;

namespace WindTurbineApi.Domain.Services
{
    public interface ISensorFactory
    {
        Task<Result<Sensor>> CreateAsync(string name);
    }
}
