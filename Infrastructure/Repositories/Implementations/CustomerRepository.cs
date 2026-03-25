using domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _db.Customers.AsNoTracking().ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _db.Customers.FindAsync(id);
    }

    public async Task AddAsync(Customer customer)
    {
        await _db.Customers.AddAsync(customer);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer != null)
        {
            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();
        }
    }
}