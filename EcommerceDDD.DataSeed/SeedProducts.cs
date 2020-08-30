using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.CurrencyExchange;
using EcommerceDDD.Domain.Products;
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

            for (int i = 0; i < 50; i++)
            {
                var price = new decimal(rand.NextDouble());
                products.Add(new Product($"Product {i}", Money.Of(price, converter.GetBaseCurrency().Name)));
            }

            await unitOfWork.ProductRepository.AddProducts(products);
            await unitOfWork.CommitAsync();
        }
    }
}
