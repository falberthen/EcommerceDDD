using MediatR;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Kafka.Consumer;
using EcommerceDDD.Core.Infrastructure.Kafka;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Orders.API.Configurations;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.IntegrationServices.Products;
using EcommerceDDD.IntegrationServices.Payments;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Orders.Application.Orders.PlacingOrder;
using EcommerceDDD.IntegrationServices.Shipments;
using EcommerceDDD.IntegrationServices;
using EcommerceDDD.Core.Testing;

var builder = WebApplication.CreateBuilder(args);

// ---- Settings
var tokenIssuerSettings = builder.Configuration.GetSection("TokenIssuerSettings");
var integrationServicesSettings = builder.Configuration.GetSection("IntegrationServicesSettings");
builder.Services.Configure<TokenIssuerSettings>(tokenIssuerSettings);
builder.Services.Configure<IntegrationServicesSettings>(integrationServicesSettings);

// ---- Services
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton(typeof(IEventConsumer), typeof(KafkaConsumer));
builder.Services.AddScoped<IEventStoreRepository<Order>, MartenRepository<Order>>();
builder.Services.AddTransient<IOrderProductsChecker, OrderProductsChecker>();
builder.Services.AddTransient<IOrdersService, OrdersService>();
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<IPaymentsService, PaymentsService>();
builder.Services.AddTransient<IShipmentsService, ShipmentsService>();
builder.Services.AddTransient<IHttpRequester, HttpRequester>();
builder.Services.AddTransient<ITokenRequester, TokenRequester>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IEventStoreRepository<DummyAggregateRoot>,
    DummyEventStoreRepository<DummyAggregateRoot>>();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddMemoryCache();

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
