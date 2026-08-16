using EcommerceDDD.QuoteManagement.Application.GettingQuoteById;

namespace EcommerceDDD.QuoteManagement.Tests.Application;

public class OwnershipGuardTests
{
	/// <summary>
	/// A quote belongs to exactly one customer, so any handler that takes a QuoteId from client
	/// input must be able to check who is asking.
	/// </summary>
	[Fact]
	public void HandlersTakingQuoteId_ShouldBeAbleToVerifyOwnership()
	{
		var unguarded = OwnershipGuardRule.FindHandlersMissingOwnershipCheck(
			applicationAssembly: typeof(OpenQuoteHandler).Assembly,
			currentUserAccessorType: typeof(IUserInfoRequester),
			ownedResourceIdTypes: [typeof(QuoteId)],
			exemptHandlers: _machineToMachineHandlers);

		Assert.True(unguarded.Count == 0,
			"These handlers accept a QuoteId but cannot verify ownership: "
			+ string.Join(", ", unguarded)
			+ ". Inject IUserInfoRequester and call EnsureCurrentCustomerOwns, or add the handler "
			+ "to the exempt list if it is only reachable machine-to-machine.");
	}

	// Reachable only through QuotesInternalController, which is restricted to the M2M role.
	// The caller is another service acting on the saga's behalf, not a customer.
	private readonly Type[] _machineToMachineHandlers =
	[
		typeof(ConfirmQuoteHandler),
		typeof(GetQuoteByIdCommandHandler)
	];
}
