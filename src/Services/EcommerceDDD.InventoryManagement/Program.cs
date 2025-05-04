var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHandlersFromType(typeof(CheckProductsInStockHandler));
services.AddHealthChecks();

// Services
services.AddScoped<IEventStoreRepository<InventoryStockUnit>, MartenRepository<InventoryStockUnit>>();
services.AddMarten(builder.Configuration,
    options => options.ConfigureProjections());

// Kiota client
services.AddApiGatewayClient(builder.Configuration);

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

// Seed products to inventory
await app.SeedInventoryCatalogAsync(builder.Configuration);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();

app.Run();
