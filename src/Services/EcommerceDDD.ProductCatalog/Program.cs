var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

builder.AddDatabaseSetup();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration, options =>
{
	options.UseServiceClientServiceLocation();
	// EF Core registers DbContextOptions as an opaque Scoped factory that codegen can't inline.
	options.CodeGeneration.AlwaysUseServiceLocationFor<ProductsDbContext>();
});
services.AddHealthChecks();

// Service clients
services.AddInventoryServiceClient(builder.Configuration);

// Services
services.AddScoped<IProducts, ProductRepository>();
services.AddScoped<ICurrencyConverter, CurrencyConverter>();

// Policies
services.AddAuthorization(options =>
{
	options.AddPolicy(Policies.CanRead, AuthPolicyBuilder.CanRead);
});

// App
var app = builder.Build();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
	app.UseSwagger(builder.Configuration);

// Seed products
await app.SeedProductCatalogAsync();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();

await app.RunAsync();