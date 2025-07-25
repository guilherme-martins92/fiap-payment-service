namespace fiap_payment_service.Dtos
{
    public class CreatePaymentDto
    {
        public required Guid CustomerId { get; set; }
        public required Guid PaymentCode { get; set; }
        public required string Description { get; set; }
        public required decimal Amount { get; set; }
    }
}