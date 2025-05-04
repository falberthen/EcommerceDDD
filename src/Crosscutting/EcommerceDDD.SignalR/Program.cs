using EcommerceDDD.Core.Infrastructure.Extensions;
using EcommerceDDD.SignalR.Hubs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

// API Versioning
services.AddApiVersioning(ApiVersions.V2);

services.AddControllers();
services.AddSwaggerGen();
services.AddEndpointsApiExplorer();
services.AddCoreInfrastructure(builder.Configuration);
services.AddHealthChecks();
services.AddSignalR();

// Order status updater
services.AddScoped<IOrderStatusUpdater, OrderStatusUpdater>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

// SignalR Hubs
app.MapHub<OrderStatusHub>(HubPaths.OrderStatushub);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks();

app.Run();
