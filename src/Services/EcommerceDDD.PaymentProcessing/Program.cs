var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration, options =>
{
    options.UseServiceClientServiceLocation();

    options.UseKafka(builder.Configuration["Kafka:ConnectionString"]!)
        .AutoProvision();

    options.PublishMessage<PaymentFinalized>().ToKafkaTopic("payments").UseDurableOutbox();
    options.PublishMessage<PaymentFailed>().ToKafkaTopic("payments").UseDurableOutbox();
    options.PublishMessage<CustomerReachedCreditLimit>().ToKafkaTopic("payments").UseDurableOutbox();
    options.PublishMessage<ProductWasOutOfStock>().ToKafkaTopic("payments").UseDurableOutbox();
});
services.AddHealthChecks();

// Service clients
services.AddInventoryServiceClient(builder.Configuration);
services.AddCustomerManagementServiceClient(builder.Configuration);

// Services
services.AddScoped<ICustomerCreditChecker, CustomerCreditChecker>();
services.AddScoped<IProductInventoryHandler, ProductInventoryHandler>();
services.AddScoped<IEventStoreRepository<Payment>, MartenRepository<Payment>>();

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
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();

await app.RunAsync();