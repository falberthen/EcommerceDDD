using EcommerceDDD.Core.CQRS;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Kafka;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure.SignalR;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Orders.Infrastructure.Projections;
using EcommerceDDD.Orders.Application.Orders.PlacingOrder;

var builder = WebApplication.CreateBuilder(args);

// ---- Configuration
builder.Services.ConfigureIntegrationHttpService(builder);
builder.Services.ConfigureCQRS();

// ---- Services
builder.Services.AddTransient<IOrderStatusBroadcaster, OrderStatusBroadcaster>();
builder.Services.AddTransient<IProductItemsChecker, ProductItemsChecker>();
builder.Services.AddTransient<IEventStoreRepository<Order>, MartenRepository<Order>>();
builder.Services.AddTransient<IEventStoreRepository<DummyAggregateRoot>, 
    DummyEventStoreRepository<DummyAggregateRoot>>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddKafkaConsumer(builder.Configuration);
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
