using EcommerceDDD.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        Task AddProduct(Product product, CancellationToken cancellationToken = default);
        Task AddProducts(List<Product> products, CancellationToken cancellationToken = default);
        Task<Product> GetProductById(ProductId id, CancellationToken cancellationToken = default);
        Task<List<Product>> GetProductsByIds(List<ProductId> ids, CancellationToken cancellationToken = default);
        Task<List<Product>> ListAllProducts(CancellationToken cancellationToken = default);
    }
}
