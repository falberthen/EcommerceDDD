var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration, options =>
{
	options.UseServiceClientServiceLocation();

	// OrderSaga is a Wolverine saga rather than a *Handler, so name it explicitly.
	options.Discovery.IncludeType<OrderSaga>();

	options.UseKafka(builder.Configuration["Kafka:ConnectionString"]!)
		.AutoProvision();

	// OrderPlaced still crosses the broker: an explicit subscription wins over
	// Wolverine's local routing, so it is not also handled in-process.
	options.PublishMessage<OrderPlaced>()
		.ToKafkaTopic("orders")
		.UseDurableOutbox();

	options.ListenToKafkaTopic("orders").UseDurableInbox();
	options.ListenToKafkaTopic("payments").UseDurableInbox();
	options.ListenToKafkaTopic("shipments").UseDurableInbox();
});
services.AddHealthChecks();

// Service clients
services.AddPaymentServiceClient(builder.Configuration);
services.AddShipmentServiceClient(builder.Configuration);
services.AddQuoteServiceClient(builder.Configuration);
services.AddOrderNotificationServiceClient(builder.Configuration);

// Services
services.AddScoped<IEventStoreRepository<Order>, MartenRepository<Order>>();
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
app.UseHealthChecks();

await app.RunAsync();