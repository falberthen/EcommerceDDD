namespace EcommerceDDD.Products.Domain;

public interface IProducts
{
    Task Add(Product product, CancellationToken cancellationToken = default);
    Task AddList(IList<Product> products, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByIds(IList<ProductId> ids, CancellationToken cancellationToken = default);
    Task<List<Product>> ListAll(CancellationToken cancellationToken = default);
}