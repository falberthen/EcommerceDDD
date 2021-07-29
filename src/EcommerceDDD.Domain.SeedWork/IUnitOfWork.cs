using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.SeedWork
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
