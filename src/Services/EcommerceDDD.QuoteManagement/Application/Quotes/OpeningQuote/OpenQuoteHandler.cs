namespace EcommerceDDD.QuoteManagement.Application.Quotes.OpeningQuote;

public class OpenQuoteHandler(
	IUserInfoRequester userInfoRequester,
	IEventStoreRepository<Quote> quoteWriteRepository,
    ICustomerOpenQuoteChecker customerOpenQuoteChecker) : ICommandHandler<OpenQuote>
{
    private readonly IEventStoreRepository<Quote> _quoteWriteRepository = quoteWriteRepository;
    private readonly ICustomerOpenQuoteChecker _customerOpenQuoteChecker = customerOpenQuoteChecker;
	private IUserInfoRequester _userInfoRequester { get; set; } = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

	public async Task Handle(OpenQuote command, CancellationToken cancellationToken)
    {
		UserInfo? userInfo = await _userInfoRequester
			.RequestUserInfoAsync();
		CustomerId customerId = CustomerId.Of(userInfo.CustomerId);

		QuoteDetails? openQuote = _customerOpenQuoteChecker
			.CheckCustomerOpenQuote(customerId);

		if (openQuote is not null)
            throw new ApplicationLogicException(
                $"The customer {customerId} has quote {openQuote.Id} open already.");

        var quote = Quote.OpenQuoteForCustomer(customerId, command.Currency);

        await _quoteWriteRepository
            .AppendEventsAsync(quote);
    }
}