namespace EcommerceDDD.Core.Infrastructure.Marten.Tests;

public class MartenQueryableStub<TResult> : List<TResult>, IMartenQueryable<TResult>
{
    private readonly IQueryProvider queryProviderMock = Substitute.For<IQueryProvider>();

    public Type ElementType => typeof(TResult);

    public Expression Expression => Expression.Constant(this);
    public QueryStatistics Statistics => throw new NotImplementedException();

    public IQueryProvider Provider
    {
        get
        {
            queryProviderMock.CreateQuery<TResult>(Arg.Any<Expression>())
                .Returns(this);
            return queryProviderMock;
        }
    }

    public Task<IReadOnlyList<TResult>> ToListAsync<TResult>(CancellationToken token)
    {
        var result = this.ToList().AsReadOnly().As<IReadOnlyList<TResult>>();
        return Task.FromResult(result);
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(CancellationToken token)
    {
        var result = this[0].As<TResult>();
        return Task.FromResult(result);
    }

    public Task<bool> AnyAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<double> AverageAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<long> CountLongAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public QueryPlan Explain(FetchType fetchType = FetchType.FetchMany, Action<IConfigureExplainExpressions>? configureExplain = null)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> FirstAsync<TResult>(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public IMartenQueryable<TResult> Include<TInclude>(Expression<Func<TResult, object>> idSource, Action<TInclude> callback) where TInclude : notnull
    {
        throw new NotImplementedException();
    }

    public IMartenQueryable<TResult> Include<TInclude>(Expression<Func<TResult, object>> idSource, IList<TInclude> list) where TInclude : notnull
    {
        throw new NotImplementedException();
    }

    public IMartenQueryable<TResult> Include<TInclude, TKey>(Expression<Func<TResult, object>> idSource, IDictionary<TKey, TInclude> dictionary)
        where TInclude : notnull
        where TKey : notnull
    {
        throw new NotImplementedException();
    }

    public Task<TResult> MaxAsync<TResult>(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> MinAsync<TResult>(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> SingleAsync<TResult>(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> SingleOrDefaultAsync<TResult>(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public IMartenQueryable<TResult> Stats(out QueryStatistics stats)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> SumAsync<TResult>(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<TResult> ToAsyncEnumerable(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}

// https://stackoverflow.com/questions/75323134/mock-setup-for-marten-idocumentsession-moq-nunit