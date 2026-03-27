using domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class BillRepository : IBillRepository
{
    private readonly AppDbContext _db;

    public BillRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Bill>> GetAllAsync()
    {
        return await _db.Bills
            .AsNoTracking()
            .Include(b => b.Customer)
            .Include(b => b.BillItems)
            .OrderByDescending(b => b.Date)
            .ToListAsync();
    }

    public async Task<Bill?> GetByIdAsync(Guid id)
    {
        return await _db.Bills.AsNoTracking()
            .Include(b => b.Customer)
            .Include(b => b.BillItems)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddAsync(Bill bill)
    {
        bill.Id = Guid.NewGuid();
        foreach (var bi in bill.BillItems)
        {
            bi.Id = Guid.NewGuid();
            bi.BillId = bill.Id;
        }

        // Customer already exists in DB — tell EF not to insert it again
        if (bill.Customer != null)
            _db.Entry(bill.Customer).State = EntityState.Unchanged;
        Console.WriteLine($"CustomerId: {bill.CustomerId}");

        foreach (var bi in bill.BillItems)
        {
            Console.WriteLine($"ItemId: {bi.ItemId}");
            Console.WriteLine($"Bill itemId: {bi.Id}");
            
        }
        await _db.Bills.AddAsync(bill);
        await _db.SaveChangesAsync();
    }
}
