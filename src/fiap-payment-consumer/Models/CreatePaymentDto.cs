namespace fiap_payment_consumer.Models
{
    public class CreatePaymentDto
    {
        public required Guid OrderId { get; set; }
        public required string Description { get; set; }
        public required decimal Amount { get; set; }
    }
}