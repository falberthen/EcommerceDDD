using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Products;

public sealed class ProductId : StronglyTypedId<ProductId>
{
    public ProductId(Guid value) : base(value)
    {
    }
}