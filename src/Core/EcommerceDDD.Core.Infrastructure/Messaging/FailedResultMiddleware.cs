namespace EcommerceDDD.Core.Infrastructure.Messaging;

/// <summary>
/// Fails a message whose handler returned a failed Result.
/// </summary>
public static class FailedResultMiddleware
{
	public static bool Applies(HandlerChain chain) =>
		chain.Handlers.Any(h => h.ReturnVariable?.VariableType == typeof(Result));

	public static void After(Result result, Envelope envelope)
	{
		// A caller of InvokeAsync<Result> reads the failure itself.
		if (result.IsFailed && envelope.ReplyRequested is null)
			throw new FailedResultException(result);
	}
}
