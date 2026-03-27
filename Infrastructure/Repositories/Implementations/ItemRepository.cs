using domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _db;

    public ItemRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Item>> GetAllAsync()
    {
        return await _db.Items.AsNoTracking().ToListAsync();
    }

    public async Task<Item?> GetByIdAsync(Guid id)
    {
        return await _db.Items.FindAsync(id);
    }

    public async Task AddAsync(Item item)
    {
        if (item.Id == null) item.Id = Guid.NewGuid();
        await _db.Items.AddAsync(item);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Item item)
    {
        _db.Items.Update(item);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item != null)
        {
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
