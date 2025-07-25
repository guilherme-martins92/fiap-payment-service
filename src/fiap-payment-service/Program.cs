using fiap_payment_service.Dtos;
using fiap_payment_service.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddScoped<IPaymentService, PaymentService>();

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

await app.RunAsync();