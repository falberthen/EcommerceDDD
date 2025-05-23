﻿namespace EcommerceDDD.ProductCatalog.Domain;

public sealed class ProductId : StronglyTypedId<Guid>
{
    public static ProductId Of(Guid value)
    {
        return new ProductId(value);
    }

    public static IEnumerable<ProductId> Of(IList<Guid>? values)
    {
		if (values is null)
			throw new ArgumentNullException(nameof(values));

        foreach (var item in values)
            yield return Of(item);
    }

    public ProductId(Guid value) : base(value)
    {
    }
}
