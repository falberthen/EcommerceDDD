namespace EcommerceDDD.Products.Infrastructure.Configurations;

public static class DatabaseSetup
{
    public static WebApplicationBuilder AddDatabaseSetup(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
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

        return builder;
    }
}