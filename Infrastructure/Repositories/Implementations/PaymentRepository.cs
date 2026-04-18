using domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class PaymentRepository : IPaymentRepository
{
    public AppDbContext _dbContext { get; set; }
    public PaymentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddPayment(Payment payment)
    {
        payment.Id = Guid.NewGuid();
        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Payment>> GetAllPayments()
    {
        var payments = await _dbContext.Payments.AsNoTracking().ToListAsync();
        return payments;
    }

    public async Task<Payment?> GetPaymentById(Guid id)
    {
        var payment = await _dbContext.Payments.FindAsync(id);
        return payment;
    }
}
