using Microsoft.EntityFrameworkCore;
using EcommerceDDD.Products.Infrastructure.Persistence;

namespace EcommerceDDD.Products.Infrastructure.Configurations;

public static class DatabaseSetup
{
    public static void AddDatabaseSetup(this IServiceCollection services, ConfigurationManager configuration)
    {
        if (services is null) 
            throw new ArgumentNullException(nameof(services));

        string connString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ProductsDbContext>(options =>
        {
            options.UseNpgsql(connString,
                npgsqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                });
        });
    }
}