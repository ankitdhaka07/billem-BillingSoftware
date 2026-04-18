using domain;

namespace Infrastructure;

public interface IPaymentRepository
{
    public Task<List<Payment>> GetAllPayments();
    public Task<Payment?> GetPaymentById(Guid id);
    public Task AddPayment(Payment payment);
}
