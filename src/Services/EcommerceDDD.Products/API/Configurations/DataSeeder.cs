using EcommerceDDD.Products.Domain;
using EcommerceDDD.Products.Infrastructure.Persistence;

namespace EcommerceDDD.Products.API.Configurations;

public static class DataSeeder
{
    public static void SeedProducts(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetService<IServiceScopeFactory>().CreateScope();

        var dbContext = serviceScope.ServiceProvider
            .GetRequiredService<ProductsDbContext>();

        if (!dbContext.Products.Any())
        {
            // Creating products
            var products = new List<Product>();
            var rand = new Random();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                var price = new decimal(rand.NextDouble());
                var product = Product.CreateNew($"Product {c}", Money.Of(price, Currency.USDollar.Code));
                products.Add(product);
            }

            dbContext.AddRange(products);
            dbContext.SaveChanges();
        }
    }
}

//https://docs.identityserver.io/en/3.1.0/quickstarts/4_entityframework.html