var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.AddDatabaseSetup();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);

// Mediator        
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetProductsHandler).Assembly));

// Services
services.AddScoped<IProducts, ProductRepository>();
services.AddScoped<ICurrencyConverter, CurrencyConverter>();

// Policies
services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyBuilder.ReadPolicy, PolicyBuilder.ReadAccess);
});

// App
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger(builder.Configuration);

// Seed products
app.SeedProducts();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
