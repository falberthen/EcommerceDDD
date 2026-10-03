namespace EcommerceDDD.Core.Infrastructure.Messaging;

/// <summary>
/// Handles a returned Result instead of cascading it as a message.
/// </summary>
public class ResultReturnPolicy : IChainPolicy
{
	private static readonly MethodInfo Handle = typeof(ResultReturnPolicy).GetMethod(nameof(HandleAsync))!;

	public void Apply(IReadOnlyList<IChain> chains, GenerationRules rules, IServiceContainer container)
	{
		foreach (var result in chains.SelectMany(chain => chain.ReturnVariablesOfType<Result>()))
			result.UseReturnAction(_ => new MethodCall(typeof(ResultReturnPolicy), Handle));
	}

	public static async ValueTask HandleAsync(Result result, MessageContext context)
	{
		// A caller of InvokeAsync<Result> receives it as the response.
		if (context.Envelope?.ReplyRequested is not null)
		{
			await context.EnqueueCascadingAsync(result);
			return;
		}

		if (result.IsFailed)
			throw new FailedResultException(result);
	}
}
