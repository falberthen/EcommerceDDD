var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHealthChecks();

// Services
services.AddScoped<IEventStoreRepository<InventoryStockUnit>, MartenRepository<InventoryStockUnit>>();
services.AddMarten(builder.Configuration,
	options => options.ConfigureProjections());

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

// Wolverine dispatches commands through a running host, so the seed has to come
// after StartAsync. RunAsync would have started and blocked in one call.
await app.StartAsync();

// Seed products to inventory (uses deterministic IDs, no external service dependency)
await app.SeedInventoryCatalogAsync();

await app.WaitForShutdownAsync();
