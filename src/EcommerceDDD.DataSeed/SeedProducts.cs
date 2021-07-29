using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.DataSeed
{
    public static class SeedProducts
    {
        public async static Task SeedData(IEcommerceUnitOfWork unitOfWork, ICurrencyConverter converter)
        {
            //Creating products
            List<Product> products = new List<Product>();
            var rand = new Random();
            
            Console.WriteLine("Adding products...\n");
            for (char c = 'A'; c <= 'Z'; c++)
            {
                var price = new decimal(rand.NextDouble());
                var productId = ProductId.Of(Guid.NewGuid());
                var product = Product.CreateNew($"Product {c}", Money.Of(price, converter.GetBaseCurrency().Code));
                products.Add(product);
                Console.WriteLine($"Added {product.Name} for {Math.Round(product.Price.Value, 2)} {product.Price.CurrencyCode}");
            }
            
            Console.WriteLine("\nProducts added.");
            await unitOfWork.Products.AddList(products);
            await unitOfWork.CommitAsync();
        }
    }
}
