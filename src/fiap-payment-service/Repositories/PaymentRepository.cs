using Amazon.DynamoDBv2.DataModel;
using fiap_payment_service.Models;

namespace fiap_payment_service.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDynamoDBContext _context;
        public PaymentRepository(IDynamoDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "DynamoDB context cannot be null.");
        }
        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment), "Payment cannot be null.");
            await _context.SaveAsync(payment);
            return payment;
        }
        public async Task<Payment> GetPaymentByIdAsync(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException("Invalid payment ID.", nameof(id));
            return await _context.LoadAsync<Payment>(id);
        }
        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            var conditions = new List<ScanCondition>();
            return await _context.ScanAsync<Payment>(conditions).GetRemainingAsync();
        }
        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment), "Payment cannot be null.");
            await _context.SaveAsync(payment);
            return payment;
        }
        public async Task<bool> DeletePaymentAsync(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException("Invalid payment ID.", nameof(id));
            var payment = await GetPaymentByIdAsync(id);
            if (payment == null) return false;
            await _context.DeleteAsync(payment);
            return true;
        }
    }
}