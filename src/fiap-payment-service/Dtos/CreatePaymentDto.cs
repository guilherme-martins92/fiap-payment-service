namespace fiap_payment_service.Dtos
{
    public class CreatePaymentDto
    {
        public required Guid OrderId { get; set; }
        public required Guid VehicleId { get; set; }
        public required string Description { get; set; }
        public required decimal Amount { get; set; }
    }
}