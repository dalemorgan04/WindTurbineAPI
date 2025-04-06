using WindTurbine.Domain.Entities;

namespace WindTurbineApi.Domain.Repositories
{
    public interface ISensorRepository
    {
        Task<Sensor?> GetByIdAsync(Guid id);
        Task<Sensor?> GetByNameAsync(string name);
        Task<IEnumerable<Sensor>> GetAllAsync();
        Task AddAsync(Sensor sensor);
        void UpdateAsync(Sensor sensor);
        Task DeleteAsync(Guid id);
    }
}
