using WindTurbine.Domain.Entities;
using WindTurbineApi.Domain.Common;
using WindTurbineApi.Domain.Repositories;

namespace WindTurbineApi.Domain.Services
{
    public class SensorFactory : ISensorFactory
    {
        private readonly ISensorRepository _sensorRepository;

        public SensorFactory(ISensorRepository sensorRepository)
        {
            _sensorRepository = sensorRepository;
        }

        public async Task<Result<Sensor>> CreateAsync(string name)
        {
            // Name duplication check - Keep this logic in the Domain layer
            if (await _sensorRepository.GetByNameAsync(name) != null)
            {
                return Result<Sensor>.Failure($"Sensor with name '{name}' already exists.");
            }

            var sensor = new Sensor { Id = Guid.NewGuid(), Name = name };
            return Result<Sensor>.Success(sensor);
        }
    }
}
