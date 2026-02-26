var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHandlersFromType(typeof(RegisterCustomerHandler));
services.AddHealthChecks();

// Service clients
services.AddIdentityServiceClient(builder.Configuration);

// Services
services.AddScoped<IEmailUniquenessChecker, EmailUniquenessChecker>();
services.AddScoped<IEventStoreRepository<Customer>, MartenRepository<Customer>>();
services.AddMarten(builder.Configuration,
	options => options.ConfigureProjections());

// Policies
services.AddAuthorization(options =>
{
	options.AddPolicy(Policies.CanRead, AuthPolicyBuilder.CanRead);
	options.AddPolicy(Policies.CanWrite, AuthPolicyBuilder.CanWrite);
});

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