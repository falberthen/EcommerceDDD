using Marten;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Quotes.API.Configurations;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Quotes.Application.Quotes.OpeningQuote;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ---- Services
builder.Services.AddInfrastructureExtension(builder.Configuration);
builder.Services.AddScoped<ICustomerOpenQuoteChecker, CustomerOpenQuoteChecker>();
builder.Services.AddScoped<IEventStoreRepository<Quote>, MartenRepository<Quote>>();
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
