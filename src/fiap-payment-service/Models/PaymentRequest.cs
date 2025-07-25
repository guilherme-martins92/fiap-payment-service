namespace fiap_payment_service.Models
{
    public class PaymentRequest
    {
        public required Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public required string PaymentMethod { get; set; } 
        public required string CustomerEmail { get; set; }
        public required string Description { get; set; }
    }
}