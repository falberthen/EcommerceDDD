namespace EcommerceDDD.Core.Testing;

/// <summary>
/// Reflection rule backing the architecture tests that keep OWASP API1 (Broken Object Level
/// Authorization) closed. A handler that takes a customer-owned resource id from client input
/// must consult the current user before touching that resource; forgetting the check on a new
/// handler is silent at runtime, so the rule turns it into a failing test instead.
/// </summary>
public static class OwnershipGuardRule
{
	/// <summary>
	/// Returns the handlers that accept one of ownedResourceIdTypes.
	/// An empty result means every user-facing handler can verify ownership.
	/// </summary>
	/// <param name="applicationAssembly">Assembly holding the command and query handlers.</param>
	/// <param name="currentUserAccessorType">Abstraction that resolves the authenticated user.</param>
	/// <param name="ownedResourceIdTypes">Id types whose resources belong to a single customer.</param>
	/// <param name="exemptHandlers">
	/// Handlers unreachable from customer-facing endpoints, and listing one is a deliberate decision, which is the point of the allowlist.
	/// </param>
	public static IReadOnlyList<string> FindHandlersMissingOwnershipCheck(
		Assembly applicationAssembly,
		Type currentUserAccessorType,
		IReadOnlyCollection<Type> ownedResourceIdTypes,
		IReadOnlyCollection<Type> exemptHandlers)
	{
		ArgumentNullException.ThrowIfNull(applicationAssembly);
		ArgumentNullException.ThrowIfNull(currentUserAccessorType);
		ArgumentNullException.ThrowIfNull(ownedResourceIdTypes);
		ArgumentNullException.ThrowIfNull(exemptHandlers);

		return applicationAssembly.GetTypes()
			.Where(type => type is { IsClass: true, IsAbstract: false })
			.Where(type => !exemptHandlers.Contains(type))
			.Where(handler => GetHandledMessages(handler)
				.Any(message => CarriesOwnedResourceId(message, ownedResourceIdTypes)))
			.Where(handler => !ReceivesCurrentUserAccessor(handler, currentUserAccessorType))
			.Select(handler => handler.Name)
			.OrderBy(name => name)
			.ToList();
	}

	private static IEnumerable<Type> GetHandledMessages(Type handler) =>
		handler.GetInterfaces()
			.Where(contract => contract.IsGenericType)
			.Where(contract =>
			{
				var definition = contract.GetGenericTypeDefinition();
				return definition == typeof(ICommandHandler<>)
					|| definition == typeof(IQueryHandler<,>);
			})
			.Select(contract => contract.GenericTypeArguments[0]);

	private static bool CarriesOwnedResourceId(Type message, IReadOnlyCollection<Type> ownedResourceIdTypes) =>
		message.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Any(property => ownedResourceIdTypes.Contains(property.PropertyType));

	private static bool ReceivesCurrentUserAccessor(Type handler, Type currentUserAccessorType) =>
		handler.GetConstructors()
			.SelectMany(constructor => constructor.GetParameters())
			.Any(parameter => parameter.ParameterType == currentUserAccessorType);
}
