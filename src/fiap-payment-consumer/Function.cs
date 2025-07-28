using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using fiap_payment_consumer.Models;
using System.Text;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace fiap_payment_consumer;

public class Function
{
    private readonly HttpClient _httpClient = new HttpClient();

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            try
            {
                context.Logger.LogInformation($"Processing message ID: {record.MessageId}");

                var messageBody = record.Body;

                // Extrai apenas o campo "detail" do envelope JSON
                var envelope = JsonDocument.Parse(messageBody);
                if (!envelope.RootElement.TryGetProperty("detail", out var detailElement))
                {
                    context.Logger.LogError($"Message ID: {record.MessageId} is missing 'detail' property.");
                    continue;
                }

                var detailJson = detailElement.GetRawText();

                // Desserializa o conteúdo de "detail" como CreatePaymentDto
                var paymentDto = JsonSerializer.Deserialize<CreatePaymentDto>(detailJson);

                if (paymentDto == null)
                {
                    context.Logger.LogError($"Failed to deserialize 'detail' for message ID: {record.MessageId}");
                    continue;
                }

                context.Logger.LogInformation($"Deserialized message ID: {record.MessageId}, Description: {paymentDto.Description}");
                context.Logger.LogInformation($"Payment DTO: {JsonSerializer.Serialize(paymentDto)}");    

                var jsonContent = new StringContent(JsonSerializer.Serialize(paymentDto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"https://lsiv6jcnil.execute-api.us-east-1.amazonaws.com/payment", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    context.Logger.LogError($"Failed to process payment for message ID: {record.MessageId}, Status Code: {response.StatusCode}");
                    continue;
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error processing message ID: {record.MessageId}, Error: {ex.Message}");
                // Aqui você pode enviar para uma DLQ ou outro mecanismo de fallback
            }
        }
    }
}