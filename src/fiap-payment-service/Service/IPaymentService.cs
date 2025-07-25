using fiap_payment_service.Dtos;
using fiap_payment_service.Models;

namespace fiap_payment_service.Service
{
    public interface IPaymentService
    {   Task<Payment> CreatePaymentAsync(CreatePaymentDto createPaymentDto);
        Task<Payment> GetPaymentByIdAsync(Guid id);
        Task<List<Payment>> GetAllPaymentsAsync();
        Task<Payment> UpdatePaymentAsync(Guid id, CreatePaymentDto updatePaymentDto);
        Task<bool> DeletePaymentAsync(Guid id);
        Task<Payment> ProcessPaymentAsync(Guid id);
    }
}