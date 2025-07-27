namespace fiap_payment_consumer.Models
{
    public class CreatePaymentDto
    {
        public required Guid OrderId { get; set; }
        public required Guid VehicleId { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
    }
}