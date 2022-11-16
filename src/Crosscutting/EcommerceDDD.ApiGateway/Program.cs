using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using EcommerceDDD.ApiGateway.SignalR.Hubs.Order;
using EcommerceDDD.Core.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ---- Ocelot
builder.Configuration.AddJsonFile("ocelot.json");

builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();
    });

// ---- Services
builder.Services.AddSignalR();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IOrderStatusUpdater, OrderStatusUpdater>();

// ---- Cors
builder.Services.AddCors(o =>
    o.AddPolicy("CorsPolicy", builder => {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(x => true)
        .WithOrigins("http://localhost:4200");
    }
));

// ---- App
var app = builder.Build();
app.UseRouting();
app.UseWebSockets();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

// ---- SignalR Hubs
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<OrderStatusHub>("orderstatushub");
});

app.UseOcelot().Wait();
app.Run();