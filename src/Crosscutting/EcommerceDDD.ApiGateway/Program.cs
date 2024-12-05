var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
const string corsPolicy = "CorsPolicy";

// Ocelot
services.AddOcelot(builder.Configuration).AddPolly();

builder.Configuration.AddOcelotWithSwaggerSupport(options =>
{
	options.Folder = "Routes";
});
services.AddSwaggerForOcelot(builder.Configuration);

services.AddSignalR();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddJwtAuthentication(builder.Configuration);
services.AddHealthChecks();

// Services
services.AddScoped<IOrderStatusUpdater, OrderStatusUpdater>();

// Cors
services.AddCors(o =>
    o.AddPolicy(corsPolicy, builder => {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(x => true)
        .WithOrigins("http://localhost:4200");
    }
));

// Swagger for ocelot
services.AddSwaggerGen();

// App
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
}

app.UseWebSockets();
app.UseRouting();
app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.UseHealthChecks();

app.UseSwaggerForOcelotUI(options =>
{
	options.PathToSwaggerGenerator = "/swagger/docs";
}).UseOcelot().Wait();

// SignalR Hubs
app.MapControllers();
app.MapHub<OrderStatusHub>("/orderstatushub");

app.Run();