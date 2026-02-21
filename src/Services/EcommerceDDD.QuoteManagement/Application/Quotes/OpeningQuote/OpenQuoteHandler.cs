namespace EcommerceDDD.QuoteManagement.Application.Quotes.OpeningQuote;

public class OpenQuoteHandler(
	IUserInfoRequester userInfoRequester,
	IEventStoreRepository<Quote> quoteWriteRepository,
    ICustomerOpenQuoteChecker customerOpenQuoteChecker
) : ICommandHandler<OpenQuote>
{
	private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;
	private readonly ICustomerOpenQuoteChecker _customerOpenQuoteChecker = customerOpenQuoteChecker;

	public async Task<Result> HandleAsync(OpenQuote command, CancellationToken cancellationToken)
    {
		UserInfo? userInfo = await userInfoRequester
			.RequestUserInfoAsync();

		if (userInfo is null)
			return Result.Fail($"The was an issue loading quote for the customer.");

		CustomerId customerId = CustomerId.Of(userInfo.CustomerId);
		QuoteDetails? openQuote = await _customerOpenQuoteChecker
			.CheckCustomerOpenQuoteAsync(customerId, cancellationToken);

		if (openQuote is not null)
            return Result.Fail($"The customer {customerId} has quote {openQuote.Id} open already.");

        var quote = Quote.OpenQuoteForCustomer(customerId, command.Currency);

        await _quoteWriteRepository
			.AppendEventsAsync(quote, cancellationToken);

        return Result.Ok();
    }
}
