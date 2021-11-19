using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Products;

public class ProductId : StronglyTypedId<ProductId>
{
    public ProductId(Guid value) : base(value)
    {
    }

    public static ProductId Of(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new BusinessRuleException("Product Id must be provided.");

        return new ProductId(productId);
    }
}
