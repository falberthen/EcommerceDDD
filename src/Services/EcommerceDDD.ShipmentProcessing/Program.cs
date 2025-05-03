var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHttpClient();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHandlersFromType(typeof(RequestShipmentHandler));
services.AddHealthChecks();

// Kiota client
services.AddApiGatewayClient(builder.Configuration);

// Services
services.AddScoped<IEventStoreRepository<Shipment>, MartenRepository<Shipment>>();

// Outbox with Debezium
services.ConfigureDebezium(builder.Configuration);

// Marten
services.AddMarten(builder.Configuration, options =>
    options.ConfigureProjections());

// Policies
services.AddAuthorization(options =>
{
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