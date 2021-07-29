using System;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Domain.Products
{
    public class Product : AggregateRoot<ProductId>
    {
        public string Name { get; private set; }
        public Money Price { get; private set; }
        public DateTime CreationDate { get; }

        public static Product CreateNew(string name, Money price)
        {
            return new Product(ProductId.Of(Guid.NewGuid()), name, price);
        }

        private Product(ProductId id, string name, Money price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be null or whitespace.", nameof(name));

            Id = id;
            Name = name;
            Price = price ?? throw new ArgumentNullException(nameof(price));
            CreationDate = DateTime.Now;
        }

        // Empty constructor for EF
        private Product(){}
    }
}
