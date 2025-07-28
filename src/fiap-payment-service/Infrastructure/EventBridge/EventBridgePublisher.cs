using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace fiap_payment_service.Infrastructure.EventBridge
{
    [ExcludeFromCodeCoverage]
    public class EventBridgePublisher : IEventPublisher
    {
        private readonly IAmazonEventBridge _eventBridge;

        private const string EventBusName = "saga-event-bus";

        public EventBridgePublisher(IAmazonEventBridge eventBridge)
        {
            _eventBridge = eventBridge;
        }

        public async Task PublishPaymentCreatedEventAsync(Guid orderId, Guid paymentId, string status)
        {
            var detail = JsonSerializer.Serialize(new
            {
                EventType = "PagamentoCriado",
                OrderId = orderId,
                Status = status,
                PaymentId = paymentId,
                Timestamp = DateTime.UtcNow
            });

            var request = new PutEventsRequest
            {
                Entries = new List<PutEventsRequestEntry>
            {
                new()
                {
                    Detail = detail,
                    DetailType = "PagamentoCriado",
                    Source = "ms-pagamentos",
                    EventBusName = EventBusName
                }
            }
            };

            var response = await _eventBridge.PutEventsAsync(request);

            if (response.FailedEntryCount > 0)
            {
                throw new InvalidOperationException("Falha ao publicar evento CompraCancelada no EventBridge.");
            }
        }
    }
}