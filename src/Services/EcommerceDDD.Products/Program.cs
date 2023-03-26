using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Products.Infrastructure.Persistence;
using EcommerceDDD.Products.Infrastructure.Configurations;
using EcommerceDDD.Products.Infrastructure.CurrencyConverter;
using EcommerceDDD.Products.Application.Products.GettingProducts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Mediator        
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetProductsHandler).Assembly));

// ---- Services
builder.Services.AddInfrastructureExtension(builder.Configuration);
builder.Services.AddScoped<IProducts, ProductRepository>();
builder.Services.AddScoped<ICurrencyConverter, CurrencyConverter>();
builder.Services.AddDatabaseSetup(builder.Configuration);

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
