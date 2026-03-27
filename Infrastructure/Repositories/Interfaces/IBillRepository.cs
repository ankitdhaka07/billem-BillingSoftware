using domain;

namespace Infrastructure;

public interface IBillRepository
{
    Task<List<Bill>> GetAllAsync();
    Task<Bill?> GetByIdAsync(Guid id);
    Task AddAsync(Bill bill);
}
