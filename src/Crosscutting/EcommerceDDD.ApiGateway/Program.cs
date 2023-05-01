var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddSignalR();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddJwtAuthentication(builder.Configuration);

// Ocelot
builder.Configuration.AddJsonFile("ocelot.json");
services.AddOcelot(builder.Configuration)
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();
    });

// Services
services.AddScoped<IOrderStatusUpdater, OrderStatusUpdater>();

// Cors
services.AddCors(o =>
    o.AddPolicy("CorsPolicy", builder => {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(x => true)
        .WithOrigins("http://localhost:4200");
    }
));

// App
var app = builder.Build();
app.UseRouting();
app.UseWebSockets();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

// SignalR Hubs
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<OrderStatusHub>("orderstatushub");
});

app.UseOcelot().Wait();
app.Run();