using Marten;
using MediatR;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Quotes.API.Configurations;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.IntegrationServices.Products;
using EcommerceDDD.Quotes.Application.OpeningQuote;
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
builder.Services.AddScoped<IEventStoreRepository<Quote>, MartenRepository<Quote>>();
builder.Services.AddScoped<IEventStoreRepository<DummyAggregateRoot>, 
    DummyEventStoreRepository<DummyAggregateRoot>>();
builder.Services.AddTransient<ICustomerQuoteOpennessChecker, CustomerQuoteOpennessChecker>();
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<IOrdersService, OrdersService>();
builder.Services.AddTransient<IHttpRequester, HttpRequester>();
builder.Services.AddTransient<ITokenRequester, TokenRequester>();
builder.Services.AddMarten(builder.Configuration, options =>
    options.ConfigureProjections());

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

// ---- App
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
