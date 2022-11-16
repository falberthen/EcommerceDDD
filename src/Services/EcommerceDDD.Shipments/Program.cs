using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Kafka;
using EcommerceDDD.Core.Infrastructure.Outbox;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Shipments.Infrastructure.Projections;
using EcommerceDDD.Shipments.Application.RequestingShipment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ---- Services
builder.Services.AddInfrastructureExtension(builder.Configuration);
builder.Services.AddTransient<IProductAvailabilityChecker, ProductAvailabilityChecker>();
builder.Services.AddScoped<IEventStoreRepository<Shipment>, MartenRepository<Shipment>>();
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