
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHealthChecks();

// Mediator        
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CheckProductsInStockHandler).Assembly));

// Services
services.AddScoped<IEventStoreRepository<InventoryStockUnit>, MartenRepository<InventoryStockUnit>>();
services.AddMarten(builder.Configuration,
    options => options.ConfigureProjections());

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

// Seed products to inventory
await app.SeedInventoryCatalogAsync(builder.Configuration);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();

app.Run();
