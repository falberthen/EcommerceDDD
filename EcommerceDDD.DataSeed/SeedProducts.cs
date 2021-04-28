using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.Services;
using EcommerceDDD.Domain.Shared;

namespace EcommerceDDD.DataSeed
{
    public static class SeedProducts
    {
        public async static Task SeedData(IEcommerceUnitOfWork unitOfWork, ICurrencyConverter converter)
        {
            //Creating products
            List<Product> products = new List<Product>();
            var rand = new Random();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                var price = new decimal(rand.NextDouble());
                var product = new Product(Guid.NewGuid(), $"Product {c}", Money.Of(price, converter.GetBaseCurrency().Code));
                products.Add(product);
                Console.WriteLine($"Added {product.Name} for {Math.Round(product.Price.Value, 2)} {product.Price.CurrencyCode}");
            }

            await unitOfWork.ProductRepository.AddRange(products);
            await unitOfWork.CommitAsync();
        }
    }
}
