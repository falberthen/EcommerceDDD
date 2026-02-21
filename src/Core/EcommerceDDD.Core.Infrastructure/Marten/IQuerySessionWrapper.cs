namespace EcommerceDDD.Core.Infrastructure.Marten;

// Wrapper around Marten's IQuerySession, exposing async methods to allow easy mocking in tests.
public interface IQuerySessionWrapper
{
	IQueryable<T> Query<T>() where T : notnull;
	Task<IReadOnlyList<T>> QueryListAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : notnull;
	Task<T?> QueryFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : notnull;
}

public class QuerySessionWrapper : IQuerySessionWrapper
{
	private readonly IQuerySession _querySession;

	public QuerySessionWrapper(IQuerySession querySession)
	{
		_querySession = querySession;
	}

	public IQueryable<T> Query<T>() where T : notnull
	{
		return _querySession.Query<T>();
	}

	public async Task<IReadOnlyList<T>> QueryListAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : notnull
	{
		return await _querySession.Query<T>().Where(predicate).ToListAsync(cancellationToken);
	}

	public async Task<T?> QueryFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : notnull
	{
		return await _querySession.Query<T>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
	}
}
