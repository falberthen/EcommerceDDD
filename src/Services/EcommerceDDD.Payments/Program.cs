using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Kafka;
using EcommerceDDD.Core.Infrastructure.Outbox;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Payments.Infrastructure.Projections;
using EcommerceDDD.Payments.Application.ProcessingPayment;
using EcommerceDDD.Payments.Application.RequestingPayment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Mediator        
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RequestPaymentHandler).Assembly));

// ---- Services
builder.Services.AddInfrastructureExtension(builder.Configuration);
builder.Services.AddScoped<ICustomerCreditChecker, CustomerCreditChecker>();
builder.Services.AddScoped<IEventStoreRepository<Payment>, MartenRepository<Payment>>();
builder.Services.AddKafkaProducer(builder.Configuration);
builder.Services.AddMarten(builder.Configuration, options =>
    options.ConfigureProjections());

// ---- Outbox
builder.Services.AddOutboxService(builder.Configuration);

// ---- App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();