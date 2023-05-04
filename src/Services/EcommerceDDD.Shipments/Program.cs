var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHttpClient();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);

// Mediator        
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RequestShipmentHandler).Assembly));

// Services
services.AddTransient<IProductAvailabilityChecker, ProductAvailabilityChecker>();
services.AddScoped<IEventStoreRepository<Shipment>, MartenRepository<Shipment>>();

// Kafka
services.AddKafkaProducer(builder.Configuration); 

// Marten
services.AddMarten(builder.Configuration, options =>
    options.ConfigureProjections());

// Outbox
services.AddOutboxService(builder.Configuration);

// Policies
services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyBuilder.M2MPolicy, PolicyBuilder.M2MAccess);
    options.AddPolicy(PolicyBuilder.WritePolicy, PolicyBuilder.WriteAccess);
});

// App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();