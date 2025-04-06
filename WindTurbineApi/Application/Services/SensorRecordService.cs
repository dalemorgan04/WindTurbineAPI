using WindTurbineApi.Application.Interfaces;
using WindTurbine.Domain.Entities;
using WindTurbineApi.Domain.Repositories;
using WindTurbineApi.Infrastructure.Repositories;
using WindTurbineApi.Domain.Common;

namespace WindTurbineApi.Application.Services
{
    public class SensorRecordService : ISensorRecordService
    {
        private readonly ISensorRecordRepository _sensorRecordRepository;
        private readonly ISensorRepository _sensorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SensorRecordService> _logger;

        public SensorRecordService(
            ISensorRecordRepository readingsRepository,
            ISensorRepository sensorRepository,
            IUnitOfWork unitOfWork,
            ILogger<SensorRecordService> logger)
        {
            _sensorRecordRepository = readingsRepository;
            _sensorRepository = sensorRepository;
            _unitOfWork = unitOfWork;
           _logger = logger;
        }

        public async Task<SensorRecord?> GetSensorRecordByIdAsync(Guid id)
        {
            return await _sensorRecordRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<SensorRecord>> GetFilteredAsync(
            string? sensorName = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            double? aboveValue = null,
            double? belowValue = null)
        {

            return await _sensorRecordRepository.GetFilteredAsync(sensorName, startDate, endDate, aboveValue, belowValue);
        }

        public async Task<SensorRecord?> CreateSensorRecordAsync(SensorRecord record)
        {
            try
            {
                await _sensorRecordRepository.AddAsync(record);
                await _unitOfWork.SaveChangesAsync();
                return record;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError($"Error occurred when creating a SensorRecord. Ex:{ex.Message}");
                return null;
            }
        }

        public async Task<Result> DeleteSensorRecordAsync(Guid id)
        {
            var recordToDelete = await _sensorRecordRepository.GetByIdAsync(id);
            if (recordToDelete == null)
            {
                return Result.NotFound();
            }

            try
            {
                await _sensorRecordRepository.DeleteAsync(recordToDelete.Id);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting data record with ID {id}: {ex.Message}");
                return Result.Failure($"An error occurred while deleting the sensor record: {ex.Message}");
            }
        }
    }
}