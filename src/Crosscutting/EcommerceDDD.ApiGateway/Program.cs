WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

// Merge the per-route files into ocelot.json and load it
builder.Configuration
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddOcelot(
		folder: "Ocelot",
		env: builder.Environment,
		mergeTo: MergeOcelotJson.ToFile,
		primaryConfigFile: "Ocelot/ocelot.json",
		reloadOnChange: true
	)
	.AddEnvironmentVariables();

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddJwtAuthentication(builder.Configuration);
services.AddHealthChecks();
services.AddSignalR();
services.AddSwaggerGen();
services.AddSwagger(builder.Configuration);
services.AddOcelot(builder.Configuration).AddQualityOfService();

// Register CORS
const string corsPolicy = "CorsPolicy";
services.AddCors(o =>
	o.AddPolicy(corsPolicy, builder =>
	{
		builder
		.AllowAnyMethod()
		.AllowAnyHeader()
		.AllowCredentials()
		.WithOrigins("http://localhost:4200");
	})
);

// OpenTelemetry
services.AddOpenTelemetryObservability("EcommerceDDD.ApiGateway");

// Register Koalesce
services.AddKoalesce(builder.Configuration);

// Build the app
var app = builder.Build();

app.UseCors(corsPolicy);
app.UseWebSockets();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHealthChecks();

// Enable Koalesce before Swagger Middleware
app.UseKoalesce();

// Enable Swagger
app.UseSwagger();

KoalesceOptions koalesceOptions;
using (var scope = app.Services.CreateScope())
{
	koalesceOptions = scope.ServiceProvider
		.GetRequiredService<IOptions<KoalesceOptions>>().Value;

	// Enable Swagger UI
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint(koalesceOptions.MergedEndpoint, koalesceOptions.Info.Title);
	});
}

app.UseOcelot().Wait();

// Run the app
await app.RunAsync();
