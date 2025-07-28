namespace fiap_payment_service.Infrastructure.EventBridge
{
    public interface IEventPublisher
    {
        Task PublishPaymentStatusEventAsync(Guid orderId, Guid paymentId, string status);       
    }
}