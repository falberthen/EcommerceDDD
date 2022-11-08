using EcommerceDDD.Core.Testing;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure.Kafka;
using EcommerceDDD.Core.Infrastructure.Outbox;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure.EventBus;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Payments.Infrastructure.Projections;
using EcommerceDDD.Payments.Application.RequestingPayment;

var builder = WebApplication.CreateBuilder(args);

// ---- Configuration
builder.Services.ConfigureIntegrationHttpService(builder);

// ---- Services
builder.Services.AddTransient<ICustomerCreditChecker, CustomerCreditChecker>();
builder.Services.AddTransient<IEventStoreRepository<Payment>, MartenRepository<Payment>>();
builder.Services.AddTransient<IEventStoreRepository<DummyAggregateRoot>, 
    DummyEventStoreRepository<DummyAggregateRoot>>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddEventDispatcher();
builder.Services.AddKafkaProducer(builder.Configuration);
builder.Services.AddMarten(builder.Configuration, options =>
    options.ConfigureProjections());

// ---- Outbox
builder.Services.AddOutboxPollingService(builder); // Comment to stop polling the database
//builder.Services.AddOutboxDebeziumConnector(builder); Uncomment to use Debezium

// ---- App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.EnsureDatabaseCreated(builder).Run();