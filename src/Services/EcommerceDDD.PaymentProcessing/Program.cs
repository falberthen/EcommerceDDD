var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHandlersFromType(typeof(RequestPaymentHandler));
services.AddHealthChecks();

// Kiota client
services.AddApiGatewayClient(builder.Configuration);

// Services
services.AddScoped<ICustomerCreditChecker, CustomerCreditChecker>();
services.AddScoped<IProductInventoryHandler, ProductInventoryHandler>();
services.AddScoped<IEventStoreRepository<Payment>, MartenRepository<Payment>>();

// Outbox with Debezium
services.ConfigureDebezium(builder.Configuration);

// Marten
services.AddMarten(builder.Configuration, options =>
    options.ConfigureProjections());

// Policies
services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.CanWrite, AuthPolicyBuilder.CanWrite);
    options.AddPolicy(Policies.CanDelete, AuthPolicyBuilder.CanDelete);
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