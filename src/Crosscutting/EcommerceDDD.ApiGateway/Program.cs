WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

// Load merged ocelot.json
builder.Configuration
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("Ocelot/ocelot.json", optional: false, reloadOnChange: true)
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
services.AddOcelot(builder.Configuration);

// Register CORS
const string corsPolicy = "CorsPolicy";
services.AddCors(o =>
	o.AddPolicy(corsPolicy, builder =>
	{
		builder
		.AllowAnyMethod()
		.AllowAnyHeader()
		.AllowCredentials()
		.SetIsOriginAllowed(x => true)
		.WithOrigins("http://localhost:4200");
	})
);


// Register Koalesce
services.AddKoalesce(builder.Configuration)
	.ForOpenAPI();

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
		c.SwaggerEndpoint(koalesceOptions.MergedOpenApiPath, koalesceOptions.Title);
	});
}

app.UseOcelot().Wait();

// Run the app
app.Run();
