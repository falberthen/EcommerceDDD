namespace EcommerceDDD.Products.Domain;

public class Product : AggregateRoot<ProductId>
{
    public string Name { get; private set; }
    public string Category { get; private set; }
    public string Description { get; private set; }
    public string ImageUrl { get; private set; }
    public Money UnitPrice { get; private set; }

    public static Product Create(ProductData productData)
    {
        var (Name, Category, Description, ImageUrl, UnitPrice) = productData
            ?? throw new ArgumentNullException(nameof(productData));

        if (string.IsNullOrWhiteSpace(Name))
            throw new BusinessRuleException("Product name cannot be null or whitespace.");
        if (string.IsNullOrWhiteSpace(Category))
            throw new BusinessRuleException("Product category cannot be null or whitespace.");
        if (string.IsNullOrWhiteSpace(Description))
            throw new BusinessRuleException("Product description cannot be null or whitespace.");        
        if (string.IsNullOrWhiteSpace(ImageUrl))
            throw new BusinessRuleException("ImageUrl cannot be null.");
        if (UnitPrice is null)
            throw new BusinessRuleException("Product unit price cannot be null.");
        
        return new Product(productData);
    }

    private Product(ProductData productData)
    {
        Id = ProductId.Of(Guid.NewGuid());
        Name = productData.Name;        
        Category = productData.Category;
        Description = productData.Description;
        ImageUrl = productData.ImageUrl;
        UnitPrice = productData.UnitPrice;
    }

    private Product(){}
}