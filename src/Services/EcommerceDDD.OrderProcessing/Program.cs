var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHealthChecks();

// Mediator        
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(PlaceOrderHandler).Assembly));

// Services
services.AddScoped<IOrderStatusBroadcaster, OrderStatusBroadcaster>();
services.AddScoped<IEventStoreRepository<Order>, MartenRepository<Order>>();
services.AddMarten(builder.Configuration, options =>
    options.ConfigureProjections());

// Kafka
services.AddKafkaConsumerAndDebezium(builder.Configuration);

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
