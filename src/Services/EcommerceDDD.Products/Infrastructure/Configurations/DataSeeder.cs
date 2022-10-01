using EcommerceDDD.Products.Domain;
using EcommerceDDD.Products.Infrastructure.Persistence;

namespace EcommerceDDD.Products.Infrastructure.Configurations;

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
            double minPrice = 1.99;
            double maxPrice = 100;

            for (char c = 'A'; c <= 'Z'; c++)
            {
                var price = new decimal(rand.NextDouble() * (maxPrice - minPrice) + minPrice);
                var productData = new ProductData($"Product {c}", Money.Of(price, Currency.USDollar.Code));
                var product = Product.Create(productData);
                products.Add(product);
            }

            dbContext.AddRange(products);
            dbContext.SaveChanges();
        }
    }
}

//https://docs.identityserver.io/en/3.1.0/quickstarts/4_entityframework.html