using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;
using EcommerceDDD.Infrastructure.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceDDD.Infrastructure.Database
{
    public static class DataSeeder
    {
        public static void SeedData(EcommerceDDDContext context)
        {
            if (!context.Products.Any())
            {
                // Creating products
                var products = new List<Product>();
                var rand = new Random();

                for (char c = 'A'; c <= 'Z'; c++)
                {
                    var price = new decimal(rand.NextDouble());
                    var productId = ProductId.Of(Guid.NewGuid());
                    var product = Product.CreateNew($"Product {c}", Money.Of(price, Currency.USDollar.Code));
                    products.Add(product);
                }

                context.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
