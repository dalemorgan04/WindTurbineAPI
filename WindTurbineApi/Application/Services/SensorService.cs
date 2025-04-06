using WindTurbine.Domain.Entities;
using WindTurbineApi.Application.Interfaces;
using WindTurbineApi.Domain.Common;
using WindTurbineApi.Domain.Repositories;
using WindTurbineApi.Domain.Services;

namespace WindTurbineApi.Application.Services
{
    public class SensorService : ISensorService
    {
        private readonly ISensorFactory _sensorFactory;
        private readonly ISensorRepository _sensorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SensorService(ISensorFactory sensorFactory, ISensorRepository sensorRepository, IUnitOfWork unitOfWork)
        {
            _sensorFactory = sensorFactory;
            _sensorRepository = sensorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Sensor?> GetSensorByIdAsync(Guid id)
        {
            return await _sensorRepository.GetByIdAsync(id);
        }

        public async Task<Sensor?> GetSensorByNameAsync(string name)
        {
            return await _sensorRepository.GetByNameAsync(name);
        }

        public async Task<IEnumerable<Sensor>> GetAllSensorsAsync()
        {
            return await _sensorRepository.GetAllAsync();
        }

        public async Task<Result<Sensor>> CreateSensorAsync(Sensor sensor)
        {

            var creationResult = await _sensorFactory.CreateAsync(sensor.Name);
            if (!creationResult.IsSuccess)
            {
                return Result<Sensor>.Failure(creationResult.Error ?? "Sensor name validation failed.");
            }

            try
            {
                await _sensorRepository.AddAsync(sensor);
                await _unitOfWork.SaveChangesAsync();
                return Result<Sensor>.Success(sensor);
            }
            catch (Exception ex)
            {
                return Result<Sensor>.Failure($"An error occurred while saving the sensor: {ex.Message}");
            }
        }

        public async Task<Result> DeleteSensorAsync(Guid id)
        {
            var sensorToDelete = await _sensorRepository.GetByIdAsync(id);
            if (sensorToDelete == null)
            {
                return Result.NotFound();
            }

            try
            {
                await _sensorRepository.DeleteAsync(sensorToDelete.Id);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting sensor with ID {id}: {ex.Message}");
                return Result.Failure($"An error occurred while deleting the sensor: {ex.Message}");
            }
        }
    }
}
