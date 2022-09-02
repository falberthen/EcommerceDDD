using MediatR;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Products.Infrastructure.Persistence;
using EcommerceDDD.Products.Infrastructure.CurrencyConverter;
using EcommerceDDD.Products.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

// ---- Services
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies()); 
builder.Services.AddScoped<IProducts, ProductRepository>();
builder.Services.AddTransient<ICurrencyConverter, CurrencyConverter>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddDatabaseSetup(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

// ---- App

// Seed products
DataSeeder.SeedProducts(app);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
