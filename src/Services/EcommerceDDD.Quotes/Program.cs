using Marten;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Quotes.API.Configurations;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Quotes.Application.Quotes.OpeningQuote;
using EcommerceDDD.Core.Infrastructure.EventBus;

var builder = WebApplication.CreateBuilder(args);

// ---- Configuration
builder.Services.ConfigureIntegrationHttpService(builder);

// ---- Services
builder.Services.AddTransient<ICustomerOpenQuoteChecker, CustomerOpenQuoteChecker>();
builder.Services.AddTransient<IEventStoreRepository<Quote>, MartenRepository<Quote>>();
builder.Services.AddTransient<IEventStoreRepository<DummyAggregateRoot>,
    DummyEventStoreRepository<DummyAggregateRoot>>();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddEventDispatcher();
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
