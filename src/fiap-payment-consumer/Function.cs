// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using fiap_payment_consumer.Models;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace fiap_payment_consumer
{
    public class Function
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var record in sqsEvent.Records)
            {
                try
                {
                    // Process each SQS message
                    context.Logger.LogInformation($"Processing message ID: {record.MessageId}");
                    var messageBody = record.Body;
                    var paymentDto = JsonSerializer.Deserialize<CreatePaymentDto>(messageBody);
                    if (paymentDto == null)
                    {
                        context.Logger.LogError($"Failed to deserialize message ID: {record.MessageId}");
                        continue;
                    }
                    // simulate request to payment service
                    var paymentServiceUrl = Environment.GetEnvironmentVariable("PAYMENT_SERVICE_URL");
                    if (string.IsNullOrEmpty(paymentServiceUrl))
                    {
                        throw new InvalidOperationException("Payment service URL is not configured.");
                    }
                    var jsonContent = new StringContent(JsonSerializer.Serialize(paymentDto), System.Text.Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{paymentServiceUrl}/payment", jsonContent);
                    if (!response.IsSuccessStatusCode)
                    {
                        context.Logger.LogError($"Failed to process payment for message ID: {record.MessageId}, Status Code: {response.StatusCode}");
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"Error processing message ID: {record.MessageId}, Error: {ex.Message}");
                    // Handle the error, e.g., send to a dead-letter queue or log it
                }
            }
        }
    }
}