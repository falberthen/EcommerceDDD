namespace EcommerceDDD.Core.Infrastructure.Marten;

// Wrapper of IQueryable, allowing it to be easily mockable
public interface IQuerySessionWrapper
{
	IQueryable<T> Query<T>();
}

public class QuerySessionWrapper : IQuerySessionWrapper
{
	private readonly IQuerySession _querySession;

	public QuerySessionWrapper(IQuerySession querySession)
	{
		_querySession = querySession;
	}

	public IQueryable<T> Query<T>()
	{
		return _querySession.Query<T>();
	}
}
