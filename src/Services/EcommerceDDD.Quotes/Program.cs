var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHttpClient();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);

// Mediator        
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(OpenQuoteHandler).Assembly));

// Services
services.AddScoped<ICustomerOpenQuoteChecker, CustomerOpenQuoteChecker>();
services.AddScoped<IEventStoreRepository<Quote>, MartenRepository<Quote>>();
services.AddMarten(builder.Configuration,
    options => options.ConfigureProjections());

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
