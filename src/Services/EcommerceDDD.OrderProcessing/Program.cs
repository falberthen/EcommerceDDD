var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration, options =>
{
	options.UseServiceClientServiceLocation();
	options.DeadLetterServiceClientRejections();

	// OrderSaga is a Wolverine saga rather than a *Handler, so name it explicitly.
	options.Discovery.IncludeType<OrderSaga>();

	options.UseKafka(builder.Configuration["Kafka:ConnectionString"]!)
		.AutoProvision();

	// payment and shipment integration events cross the broker.
	options.ListenToKafkaTopic("payments").UseDurableInbox();
	options.ListenToKafkaTopic("shipments").UseDurableInbox();

	// The saga steps run over local queues.
	// Durable ones persist each message in the same transaction as the state change that produced it,
	// so a crash can't strand an order between steps, and exhausted retries dead-letter instead of vanishing from memory.
	options.Policies.UseDurableLocalQueues();
});
services.AddHealthChecks();
services.AddWolverineHttp();

// Service clients
services.AddPaymentServiceClient(builder.Configuration);
services.AddShipmentServiceClient(builder.Configuration);
services.AddQuoteServiceClient(builder.Configuration);
services.AddOrderNotificationServiceClient(builder.Configuration);
services.AddInventoryServiceClient(builder.Configuration);

// Services
services.AddScoped<IProductInventoryHandler, ProductInventoryHandler>();
services.AddScoped<IEventStoreRepository<Order>, MartenRepository<Order>>();
services.AddSingleton<OrderMetrics>();
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
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
	app.UseSwagger(builder.Configuration);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Wolverine's built-in dead-letter admin (query/replay/delete by id), behind auth.
app.MapGroup("")
	.RequireAuthorization(Policies.CanWrite)
	.MapDeadLettersEndpoints()
	.ExcludeFromDescription();

app.UseHealthChecks();

await app.RunAsync();