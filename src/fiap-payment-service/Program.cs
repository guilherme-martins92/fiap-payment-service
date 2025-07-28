using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.EventBridge;
using fiap_payment_service.Dtos;
using fiap_payment_service.Infrastructure.EventBridge;
using fiap_payment_service.Repositories;
using fiap_payment_service.Service;

var builder = WebApplication.CreateBuilder(args);


// Add AWS Lambda support. When running the application as an AWS Serverless application, Kestrel is replaced
// with a Lambda function contained in the Amazon.Lambda.AspNetCoreServer package, which marshals the request into the ASP.NET Core hosting framework.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddScoped<IEventPublisher, EventBridgePublisher>();

builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    return new AmazonDynamoDBClient();
});

builder.Services.AddSingleton<IAmazonEventBridge>(sp =>
{
    return new AmazonEventBridgeClient();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/payment", async (CreatePaymentDto input, IPaymentService paymentService) =>
{
    var result = await paymentService.CreatePaymentAsync(input);
    return Results.Ok(result);
})
.WithName("CreatePayment")
.WithOpenApi();

app.MapGet("/payment/{id:guid}", async (Guid id, IPaymentService paymentService) =>
{
    var result = await paymentService.GetPaymentByIdAsync(id);
    return Results.Ok(result);
})
.WithName("GetPayment")
.WithOpenApi();

app.MapGet("/payments", async (IPaymentService paymentService) =>
{
    var result = await paymentService.GetAllPaymentsAsync();
    return Results.Ok(result);
})
 .WithName("GetAllPayments")
 .WithOpenApi();

app.MapPut("/payment/process/{id:guid}", async (Guid id, IPaymentService paymentService) =>
{
    var result = await paymentService.ProcessPaymentAsync(id);
    return Results.Ok(result);
})
.WithName("ProcessPayment")
.WithDescription("Processa o pagamento através do código.")
.WithOpenApi();

await app.RunAsync();