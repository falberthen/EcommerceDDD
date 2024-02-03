using EcommerceDDD.ShipmentProcessing.Infrastructure.InventoryHandling;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHttpClient();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHealthChecks();

// Mediator        
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RequestShipmentHandler).Assembly));

// Services
services.AddTransient<IProductInventoryHandler, ProductInventoryHandler>();
services.AddScoped<IEventStoreRepository<Shipment>, MartenRepository<Shipment>>();

// Outbox with Debezium
services.ConfigureDebezium(builder.Configuration);

// Marten
services.AddMarten(builder.Configuration, options =>
    options.ConfigureProjections());

// Policies
services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.M2MAccess, AuthPolicyBuilder.M2MAccess);
    options.AddPolicy(Policies.CanRead, AuthPolicyBuilder.CanRead);
    options.AddPolicy(Policies.CanWrite, AuthPolicyBuilder.CanWrite);
});

// App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();

app.Run();