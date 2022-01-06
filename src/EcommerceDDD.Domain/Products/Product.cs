using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Domain.Products;

public class Product : AggregateRoot<ProductId>
{
    public string Name { get; private set; }
    public Money Price { get; private set; }
    public DateTime CreationDate { get; }

    public static Product CreateNew(string name, Money price)
    {
        var productId = new ProductId(Guid.NewGuid());
        return new Product(productId, name, price);
    }

    private Product(ProductId id, string name, Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("Product name cannot be null or whitespace.");

        Id = id;
        Name = name;
        Price = price ?? throw new BusinessRuleException("Product price cannot be null or whitespace.");
        CreationDate = DateTime.Now;
    }

    // Empty constructor for EF
    private Product(){}
}