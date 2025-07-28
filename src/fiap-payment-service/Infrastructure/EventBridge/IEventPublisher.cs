namespace fiap_payment_service.Infrastructure.EventBridge
{
    public interface IEventPublisher
    {
        Task PublishPaymentCreatedEventAsync(Guid orderId, Guid paymentId, string status);
    }
}