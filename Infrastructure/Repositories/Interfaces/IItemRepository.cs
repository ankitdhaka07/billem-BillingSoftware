using domain;

namespace Infrastructure;

public interface IItemRepository
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetByIdAsync(Guid id);
    Task AddAsync(Item item);
    Task UpdateAsync(Item item);
    Task DeleteAsync(Guid id);
}
