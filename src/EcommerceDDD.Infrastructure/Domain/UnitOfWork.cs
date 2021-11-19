global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.EntityFrameworkCore;
global using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Infrastructure.Domain;

public abstract class UnitOfWork<TDB> : IUnitOfWork
    where TDB : DbContext
{
    protected readonly TDB DbContext;

    protected UnitOfWork(TDB dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<bool> CommitAsync(CancellationToken cancellationToken)
    {
        await StoreEvents(cancellationToken);
        return await DbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    protected abstract Task StoreEvents(CancellationToken cancellationToken);
}