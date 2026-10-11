namespace EcommerceDDD.Core.Infrastructure.OpenTelemetry;

/// <summary>
/// Tags the handler span of every command and event carrying an OrderId with "order.id".
/// </summary>
public class OrderIdTagPolicy : IChainPolicy
{
	public const string OrderIdTag = "order.id";
	private const string OrderIdMember = "OrderId";

	public void Apply(IReadOnlyList<IChain> chains, GenerationRules rules, IServiceContainer container)
	{
		foreach (var chain in chains)
		{
			var messageType = chain.InputType();
			if (messageType is null
				|| !(typeof(ICommand).IsAssignableFrom(messageType) || typeof(INotification).IsAssignableFrom(messageType)))
				continue;

			var orderId = messageType.GetProperty(OrderIdMember);
			if (orderId is null)
				continue;

			// Replaces the saga identity audit, which uses the member name as the tag.
			chain.AuditedMembers.RemoveAll(member => member.Member.Name == OrderIdMember);
			chain.AuditedMembers.Add(new AuditedMember(orderId, OrderIdMember, OrderIdTag));
		}
	}
}
