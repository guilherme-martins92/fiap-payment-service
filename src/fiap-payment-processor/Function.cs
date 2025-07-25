using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using fiap_payment_processor.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace fiap_payment_processor;

[ExcludeFromCodeCoverage]
public class Function
{
    private const string EventBusName = "saga-event-bus";

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            try
            {
                var paymentRequest = JsonSerializer.Deserialize<PaymentRequest>(record.Body);
                if (paymentRequest == null)
                {
                    context.Logger.LogLine("Invalid payment request received.");
                    continue;
                }

                var response = await ProcessPaymentAsync(paymentRequest);

                context.Logger.LogInformation($"Pagamento processado: {response}");
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error processing record: {ex.Message}");
                throw;
            }
        }
    }

    public async Task<string> ProcessPaymentAsync(PaymentRequest paymentRequest)
    {
        string teste = string.Empty;
        try
        {
            await PublishPaymentMadeAsync(paymentRequest.OrderId);
            return $"Pagamento do pedido {paymentRequest.OrderId} processado com sucesso!";
        }
        catch (Exception ex)
        {
            return $"Erro ao processar pagamento: {ex.Message}" + teste;
        }
    }

    public async Task PublishPaymentMadeAsync(Guid orderId)
    {
        var client = new AmazonEventBridgeClient();

        var detail = JsonSerializer.Serialize(new
        {
            EventType = "PagamentoRealizado",
            OrderId = orderId,
            Timestamp = DateTime.UtcNow
        });

        var request = new PutEventsRequest
        {
            Entries = new List<PutEventsRequestEntry>
            {
                new()
                {
                    Detail = detail,
                    DetailType = "PagamentoRealizado",
                    Source = "ms.payment",
                    EventBusName = EventBusName
                }
            }
        };

        var response = await client.PutEventsAsync(request);

        if (response.FailedEntryCount > 0)
        {
            throw new InvalidOperationException("Falha ao publicar evento PagamentoRealizado no EventBridge.");
        }
    }
}