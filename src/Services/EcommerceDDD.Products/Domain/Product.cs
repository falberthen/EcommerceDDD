using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Products.Domain;

public class Product : AggregateRoot<ProductId>
{
    public string Name { get; private set; }
    public Money UnitPrice { get; private set; }

    public static Product CreateNew(string name, Money unitPrice)
    {
        return new Product(name, unitPrice);
    }

    private Product(string name, Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be null or whitespace.");

        Id = ProductId.Of(Guid.NewGuid());
        Name = name;
        UnitPrice = price ?? throw new DomainException("Product price cannot be null or whitespace.");
    }

    private Product(){}
}