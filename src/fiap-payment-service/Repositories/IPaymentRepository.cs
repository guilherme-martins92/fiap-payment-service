using fiap_payment_service.Models;

namespace fiap_payment_service.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByIdAsync(Guid id);
        Task<List<Payment>> GetAllPaymentsAsync();
        Task<Payment> UpdatePaymentAsync(Payment payment);
        Task<bool> DeletePaymentAsync(Guid id);
    }
}