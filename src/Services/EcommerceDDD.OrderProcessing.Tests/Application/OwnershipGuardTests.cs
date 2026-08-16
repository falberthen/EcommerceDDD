namespace EcommerceDDD.OrderProcessing.Tests.Application;

public class OwnershipGuardTests
{
	/// <summary>
	/// Orders and quotes belong to exactly one customer, so any handler that takes one of their ids
	/// from client input must be able to check who is asking.
	/// </summary>
	[Fact]
	public void HandlersTakingOrderOrQuoteId_ShouldBeAbleToVerifyOwnership()
	{
		var unguarded = OwnershipGuardRule.FindHandlersMissingOwnershipCheck(
			applicationAssembly: typeof(PlaceOrderHandler).Assembly,
			currentUserAccessorType: typeof(IUserInfoRequester),
			ownedResourceIdTypes: [typeof(OrderId), typeof(QuoteId)],
			exemptHandlers: _sagaDrivenHandlers);

		Assert.True(unguarded.Count == 0,
			"These handlers accept an OrderId or QuoteId but cannot verify ownership: "
			+ string.Join(", ", unguarded)
			+ ". Inject IUserInfoRequester and call EnsureCurrentCustomerOwns, or add the handler "
			+ "to the exempt list if it is only reachable from the saga.");
	}

	// Driven by OrderSaga in reaction to domain and integration events, never by a customer
	// request. There is no authenticated user in scope, so ownership was already settled upstream
	// when the order was placed.
	private readonly Type[] _sagaDrivenHandlers =
	[
		typeof(ProcessOrderHandler),
		typeof(CancelOrderHandler),
		typeof(RecordPaymentHandler),
		typeof(RequestPaymentHandler),
		typeof(RequestCancelPaymentHandler),
		typeof(RecordShipmentHandler),
		typeof(RequestShipmentHandler)
	];
}
