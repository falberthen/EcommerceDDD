using EcommerceDDD.Domain.Core.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        Task Add(Product product, CancellationToken cancellationToken = default);
        Task AddRange(List<Product> products, CancellationToken cancellationToken = default);
        Task<Product> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<List<Product>> GetByIds(List<Guid> ids, CancellationToken cancellationToken = default);
        Task<List<Product>> ListAll(CancellationToken cancellationToken = default);
    }
}
