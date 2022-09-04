using MediatR;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Customers.Domain;
using EcommerceDDD.Customers.API.Configurations;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Customers.Application.RegisteringCustomer;
using EcommerceDDD.Core.EventBus;

var builder = WebApplication.CreateBuilder(args);

// ---- Settings
var tokenIssuerSettings = builder.Configuration.GetSection("TokenIssuerSettings");
builder.Services.Configure<TokenIssuerSettings>(tokenIssuerSettings);

// ---- Services
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICustomerUniquenessChecker, CustomerUniquenessChecker>();
builder.Services.AddTransient<IHttpRequester, HttpRequester>();
builder.Services.AddScoped<IEventStoreRepository<Customer>, MartenRepository<Customer>>();
builder.Services.AddScoped<IEventStoreRepository<DummyAggregateRoot>, 
    DummyEventStoreRepository<DummyAggregateRoot>>();
builder.Services.AddMarten(builder.Configuration, options => options.ConfigureProjections());
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

builder.Services.AddHttpClient();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger(builder.Configuration);

// ---- App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
