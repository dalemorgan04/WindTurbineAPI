using Microsoft.EntityFrameworkCore;
using WindTurbine.Domain.Entities;
using WindTurbineApi.Domain.Repositories;
using WindTurbineApi.Infrastructure.Persistence;

namespace WindTurbineApi.Infrastructure.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;

        public SensorRepository(ApplicationDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<Sensor?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Sensors.FindAsync(id);
        }

        public async Task<IEnumerable<Sensor>> GetAllAsync()
        {
            return await _dbContext.Sensors.ToListAsync();
        }

        public async Task<Sensor?> GetByNameAsync(string name)
        {
            return await _dbContext.Sensors.FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task AddAsync(Sensor sensor)
        {
            await _dbContext.Sensors.AddAsync(sensor);
        }

        public void UpdateAsync(Sensor sensor)
        {
            _dbContext.Sensors.Update(sensor);
        }

        public async Task DeleteAsync(Guid id)
        {
            var sensor = await _dbContext.Sensors.FindAsync(id);
            if (sensor != null)
            {
                _dbContext.Sensors.Remove(sensor);
            }
        }
    }
}
