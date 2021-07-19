using EcommerceDDD.Domain.SeedWork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Products
{
    public interface IProducts : IRepository<Product>
    {
        Task Add(Product product, CancellationToken cancellationToken = default);
        Task AddList(List<Product> products, CancellationToken cancellationToken = default);
        Task<Product> GetById(ProductId id, CancellationToken cancellationToken = default);
        Task<List<Product>> GetByIds(List<ProductId> ids, CancellationToken cancellationToken = default);
        Task<List<Product>> ListAll(CancellationToken cancellationToken = default);
    }
}
