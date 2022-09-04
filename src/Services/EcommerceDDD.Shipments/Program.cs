using MediatR;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure.Kafka.Producer;
using EcommerceDDD.Core.Infrastructure.Kafka;
using EcommerceDDD.Shipments.Domain;

var builder = WebApplication.CreateBuilder(args);

// ---- Services
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton(typeof(IEventProducer), typeof(KafkaProducer));
builder.Services.AddScoped<IEventStoreRepository<Shipment>, MartenRepository<Shipment>>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IEventStoreRepository<DummyAggregateRoot>, 
    DummyEventStoreRepository<DummyAggregateRoot>>();

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