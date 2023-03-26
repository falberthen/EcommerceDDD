using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Customers.Domain;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Customers.Application.RegisteringCustomer;
using EcommerceDDD.Customers.Infrastructure.Projections;
using EcommerceDDD.Customers.Api.Application.RegisteringCustomer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Mediator        
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(RegisterCustomerHandler).Assembly));

// ---- Services
builder.Services.AddInfrastructureExtension(builder.Configuration);
builder.Services.AddScoped<IEmailUniquenessChecker, EmailUniquenessChecker>();
builder.Services.AddScoped<IEventStoreRepository<Customer>, MartenRepository<Customer>>();
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
