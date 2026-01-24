var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHandlersFromType(typeof(OrderSaga));
services.AddHealthChecks();

// Kiota clients
services.AddKiotaClient<PaymentProcessingClient>(builder.Configuration["Services:PaymentProcessing"]);
services.AddKiotaClient<ShipmentProcessingClient>(builder.Configuration["Services:ShipmentProcessing"]);
services.AddKiotaClient<QuoteManagementClient>(builder.Configuration["Services:QuoteManagement"]);
services.AddKiotaClient<SignalRClient>(builder.Configuration["Services:SignalRClient"]);

// Services
services.AddScoped<IEventStoreRepository<Order>, MartenRepository<Order>>();
services.AddMarten(builder.Configuration, options =>
	options.ConfigureProjections());

// Kafka
services.AddKafkaConsumerAndDebezium(builder.Configuration);

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

await app.RunAsync();