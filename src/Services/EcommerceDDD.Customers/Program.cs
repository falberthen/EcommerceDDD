using EcommerceDDD.Core.CQRS;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Customers.Domain;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Customers.Application.RegisteringCustomer;
using EcommerceDDD.Customers.Infrastructure.Projections;

var builder = WebApplication.CreateBuilder(args);

// ---- Configuration
builder.Services.ConfigureIntegrationHttpService(builder);
builder.Services.ConfigureCQRS();

// ---- Services
builder.Services.AddTransient<IEmailUniquenessChecker, EmailUniquenessChecker>();
builder.Services.AddTransient<IEventStoreRepository<Customer>, MartenRepository<Customer>>();
builder.Services.AddTransient<IEventStoreRepository<DummyAggregateRoot>,
    DummyEventStoreRepository<DummyAggregateRoot>>();
builder.Services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();

builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddMarten(builder.Configuration,
    options => options.ConfigureProjections());

// ---- App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
