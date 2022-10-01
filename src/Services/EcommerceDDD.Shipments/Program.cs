using EcommerceDDD.Core.CQRS;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure.Kafka;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Shipments.Infrastructure.Projections;
using EcommerceDDD.Shipments.Application.ShippingPackage;

var builder = WebApplication.CreateBuilder(args);

// ---- Configuration
builder.Services.ConfigureIntegrationHttpService(builder);
builder.Services.ConfigureCQRS();

// ---- Services
builder.Services.AddTransient<IProductAvailabilityChecker, ProductAvailabilityChecker>();
builder.Services.AddTransient<IEventStoreRepository<Shipment>, MartenRepository<Shipment>>();
builder.Services.AddTransient<IEventStoreRepository<DummyAggregateRoot>,
    DummyEventStoreRepository<DummyAggregateRoot>>();

builder.Services.AddKafkaProducer(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddMarten(builder.Configuration, options =>
    options.ConfigureProjections());

// ---- App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();