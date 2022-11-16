using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Kafka;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.SignalR;
using EcommerceDDD.Orders.Infrastructure.Projections;
using EcommerceDDD.Orders.Application.Orders.PlacingOrder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ---- Services
builder.Services.AddInfrastructureExtension(builder.Configuration);
builder.Services.AddScoped<IOrderStatusBroadcaster, OrderStatusBroadcaster>();
builder.Services.AddScoped<IProductItemsMapper, ProductItemsMapper>();
builder.Services.AddScoped<IEventStoreRepository<Order>, MartenRepository<Order>>();
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
