using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using EcommerceDDD.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Infrastructure.Database.Context;

namespace EcommerceDDD.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = scope.ServiceProvider.GetService<EcommerceDDDContext>();
            DataSeeder.SeedData(context);
        }

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();                    
            });        
}
