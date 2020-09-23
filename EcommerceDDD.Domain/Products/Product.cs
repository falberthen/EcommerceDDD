using System;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Shared;

namespace EcommerceDDD.Domain.Products
{
    public class Product : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public Money Price { get; private set; }
        public DateTime CreationDate { get; }

        public Product(Guid id, string name, Money price)
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
