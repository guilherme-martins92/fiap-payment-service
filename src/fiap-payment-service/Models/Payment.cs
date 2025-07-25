namespace fiap_payment_service.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public required Guid CustomerId { get; set; }
        public required Guid PaymentCode { get; set; }
        public required string Description { get; set; }
        public required decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; set; }
    }
}