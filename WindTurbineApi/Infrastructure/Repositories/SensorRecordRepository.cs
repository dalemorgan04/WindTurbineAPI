using Microsoft.EntityFrameworkCore;
using WindTurbine.Domain.Entities;
using WindTurbineApi.Domain.Repositories;
using WindTurbineApi.Infrastructure.Persistence;

namespace WindTurbineApi.Infrastructure.Repositories
{
    public class SensorRecordRepository : ISensorRecordRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;

        public SensorRecordRepository(ApplicationDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<SensorRecord?> GetByIdAsync(Guid id)
        {
            return await _dbContext.SensorRecords.FindAsync(id);
        }
        public async Task<SensorRecord?> GetByNameAsync(string name)
        {
            return await _dbContext.SensorRecords
                .FirstOrDefaultAsync(sr => sr.Sensor.Name == name);
        }

        public async Task<IEnumerable<SensorRecord>> GetAllAsync()
        {
            return await _dbContext.SensorRecords.ToListAsync();
        }

        public async Task<IEnumerable<SensorRecord>> GetFilteredAsync(
               string? sensorName = null,
               DateTime? startDate = null,
               DateTime? endDate = null,
               double? aboveValue = null,
               double? belowValue = null)
        {
            IQueryable<SensorRecord> query = _dbContext.SensorRecords;

            if (!string.IsNullOrEmpty(sensorName))
            {
                query = query.Where(r => r.Sensor.Name == sensorName);
            }

            if (startDate.HasValue)
            {
                query = query.Where(r => r.Timestamp >= startDate.Value.ToUniversalTime());
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.Timestamp <= endDate.Value.ToUniversalTime());
            }

            if (aboveValue.HasValue)
            {
                query = query.Where(r => r.Reading.Value > aboveValue.Value);
            }

            if (belowValue.HasValue)
            {
                query = query.Where(r => r.Reading.Value < belowValue.Value);
            }

            query = query.Include(r => r.Sensor);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<SensorRecord>> GetAllByDateTimeRangeAsync(DateTime fromDateTime, DateTime toDateTime)
        {
            return await _dbContext.SensorRecords
                .Where(sr => sr.Timestamp >= fromDateTime && sr.Timestamp <= toDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<SensorRecord>> GetAllByTemperatureRangeAsync(double fromTemperature, double toTemperature)
        {
            return await _dbContext.SensorRecords
                .Where(sr => sr.Reading.Value >= fromTemperature && sr.Reading.Value <= toTemperature) // Assuming you have a Temperature property
                .ToListAsync();
        }

        public async Task AddAsync(SensorRecord reading)
        {
            await _dbContext.SensorRecords.AddAsync(reading);
        }

        public void UpdateAsync(SensorRecord reading)
        {
            _dbContext.SensorRecords.Update(reading);
        }

        public async Task DeleteAsync(Guid id)
        {
            var reading = await _dbContext.SensorRecords.FindAsync(id);
            if (reading != null)
            {
                _dbContext.SensorRecords.Remove(reading);
            }
        }
    }
}
