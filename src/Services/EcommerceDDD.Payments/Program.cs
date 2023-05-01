var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);

// Mediator        
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RequestPaymentHandler).Assembly));

// Services
services.AddScoped<ICustomerCreditChecker, CustomerCreditChecker>();
services.AddScoped<IEventStoreRepository<Payment>, MartenRepository<Payment>>();

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
    options.AddPolicy(PolicyBuilder.ReadPolicy, PolicyBuilder.ReadAccess);
    options.AddPolicy(PolicyBuilder.WritePolicy, PolicyBuilder.WriteAccess);
    options.AddPolicy(PolicyBuilder.DeletePolicy, PolicyBuilder.DeleteAccess);
});

// App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();