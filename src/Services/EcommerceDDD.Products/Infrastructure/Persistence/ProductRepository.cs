namespace EcommerceDDD.Products.Infrastructure.Persistence;

public class ProductRepository : IProducts
{
    private readonly ProductsDbContext _context;

    public ProductRepository(ProductsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task Add(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public async Task AddList(IList<Product> products, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddRangeAsync(products, cancellationToken);
    }

    public async Task<List<Product>> GetByIds(IList<ProductId> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Products.Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> ListAll(CancellationToken cancellationToken = default)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }
}
