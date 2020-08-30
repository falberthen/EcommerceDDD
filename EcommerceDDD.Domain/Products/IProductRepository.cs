using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Products
{
    public interface IProductRepository
    {
        Task AddProduct(Product product, CancellationToken cancellationToken = default);
        Task AddProducts(List<Product> products, CancellationToken cancellationToken = default);
        Task<Product> GetProductById(Guid id, CancellationToken cancellationToken = default);
        Task<List<Product>> GetProductsByIds(List<Guid> ids, CancellationToken cancellationToken = default);
        Task<List<Product>> ListAllProducts(CancellationToken cancellationToken = default);
    }
}
