using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Products.Domain;

public class Product : AggregateRoot<ProductId>
{
    public string Name { get; private set; }
    public Money UnitPrice { get; private set; }

    public static Product Create(ProductData productData)
    {
        var (Name, UnitPrice) = productData
            ?? throw new ArgumentNullException(nameof(productData));

        if (string.IsNullOrWhiteSpace(Name))
            throw new BusinessRuleException("Product name cannot be null or whitespace.");

        if (UnitPrice is null)
            throw new BusinessRuleException("Product unit price cannot be null.");

        return new Product(productData);
    }

    private Product(ProductData productData)
    {
        Id = ProductId.Of(Guid.NewGuid());
        Name = productData.Name;
        UnitPrice = productData.UnitPrice;
    }

    private Product(){}
}