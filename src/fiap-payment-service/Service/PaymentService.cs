using fiap_payment_service.Constants;
using fiap_payment_service.Dtos;
using fiap_payment_service.Models;
using fiap_payment_service.Repositories;

namespace fiap_payment_service.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(ILogger<PaymentService> logger, IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _paymentRepository = paymentRepository;
        }

        public async Task<Payment> CreatePaymentAsync(CreatePaymentDto createPaymentDto)
        {
            try
            {
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    CustomerId = createPaymentDto.CustomerId,                    
                    Description = createPaymentDto.Description,
                    Amount = createPaymentDto.Amount,
                    Status = PaymentStatus.Pending,
                    UpdateAt = DateTime.UtcNow
                };

                await _paymentRepository.CreatePaymentAsync(payment);
                _logger.LogInformation("Payment created successfully with ID: {PaymentId}", payment.Id);
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment: {Message}", ex.Message);
                throw new Exception("An error occurred while creating the payment.", ex);
            }
        }

        public async Task<Payment> GetPaymentByIdAsync(Guid id)
        {
            try
            {
                return await _paymentRepository.GetPaymentByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment by ID: {PaymentId}, Message: {Message}", id, ex.Message);
                throw new Exception($"An error occurred while fetching the payment with ID: {id}", ex);
            }
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            try
            {
                return await _paymentRepository.GetAllPaymentsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all payments: {Message}", ex.Message);
                throw new Exception("An error occurred while fetching all payments.", ex);
            }
        }

        public async Task<Payment> UpdatePaymentAsync(Guid id, CreatePaymentDto updatePaymentDto)
        {
            try
            {
                var existingPayment = await _paymentRepository.GetPaymentByIdAsync(id);
                if (existingPayment == null)
                {
                    throw new Exception($"Payment with ID: {id} not found.");
                }
                existingPayment.Description = updatePaymentDto.Description;
                existingPayment.Amount = updatePaymentDto.Amount;
                existingPayment.UpdateAt = DateTime.UtcNow;
                return await _paymentRepository.UpdatePaymentAsync(existingPayment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment with ID: {PaymentId}, Message: {Message}", id, ex.Message);
                throw new Exception($"An error occurred while updating the payment with ID: {id}", ex);
            }
        }

        public async Task<bool> DeletePaymentAsync(Guid id)
        {
            // Simulate deleting payment logic
            await Task.Delay(100); // Simulating async operation
            return true; // Assume deletion was successful
        }

        public async Task<Payment> ProcessPaymentAsync(Guid id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentByIdAsync(id);
                if (payment == null)
                {
                    throw new Exception($"Payment with ID: {id} not found.");
                }
                // Simulate payment processing logic
                await Task.Delay(100); // Simulating async payment processing
                payment.Status = PaymentStatus.Completed;
                payment.UpdateAt = DateTime.UtcNow;
                await _paymentRepository.UpdatePaymentAsync(payment);
                _logger.LogInformation("Payment processed successfully with ID: {PaymentId}", payment.Id);
                return payment;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment with ID: {PaymentId}, Message: {Message}", id, ex.Message);
                throw new Exception($"An error occurred while processing the payment with ID: {id}", ex);
            }
        }
    }
}