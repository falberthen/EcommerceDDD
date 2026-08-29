var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddHttpClient();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration, options =>
{
    options.UseKafka(builder.Configuration["Kafka:ConnectionString"]!)
        .AutoProvision();

    options.PublishMessage<ShipmentFinalized>().ToKafkaTopic("shipments").UseDurableOutbox();
    options.PublishMessage<ShipmentFailed>().ToKafkaTopic("shipments").UseDurableOutbox();
});
services.AddHealthChecks();

// Services
services.AddScoped<IEventStoreRepository<Shipment>, MartenRepository<Shipment>>();

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
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();

await app.RunAsync();