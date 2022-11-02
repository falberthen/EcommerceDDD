using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Products.Infrastructure.Persistence;
using EcommerceDDD.Products.Infrastructure.Configurations;
using EcommerceDDD.Products.Infrastructure.CurrencyConverter;
using EcommerceDDD.Core.Infrastructure.EventBus;

var builder = WebApplication.CreateBuilder(args);

// ---- Services
builder.Services.AddScoped<IProducts, ProductRepository>();
builder.Services.AddScoped<ICurrencyConverter, CurrencyConverter>();
builder.Services.AddTransient<IEventStoreRepository<DummyAggregateRoot>,
    DummyEventStoreRepository<DummyAggregateRoot>>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddDatabaseSetup(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddEventDispatcher();

// ---- App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

// Seed products
DataSeeder.SeedProducts(app);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
