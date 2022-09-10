using MediatR;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Core.Infrastructure.Kafka;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure.Kafka.Producer;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.IntegrationServices.Customers;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.IntegrationServices;

var builder = WebApplication.CreateBuilder(args);

// ---- Settings
var tokenIssuerSettings = builder.Configuration.GetSection("TokenIssuerSettings");
var integrationServicesSettings = builder.Configuration.GetSection("IntegrationServicesSettings");
builder.Services.Configure<TokenIssuerSettings>(tokenIssuerSettings);
builder.Services.Configure<IntegrationServicesSettings>(integrationServicesSettings);

// ---- Services
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton(typeof(IEventProducer), typeof(KafkaProducer));
builder.Services.AddScoped<IEventStoreRepository<Payment>, MartenRepository<Payment>>();
builder.Services.AddTransient<ICustomersService, CustomersService>();
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
builder.Services.AddKafkaProducer(builder.Configuration);
builder.Services.AddMarten(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

// ---- App
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();