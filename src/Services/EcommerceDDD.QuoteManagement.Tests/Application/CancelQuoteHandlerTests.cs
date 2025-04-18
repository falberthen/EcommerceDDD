using EcommerceDDD.Core.Infrastructure.Identity;

namespace EcommerceDDD.QuoteManagement.Tests.Application;

public class CancelQuoteHandlerTests
{
	[Fact]
	public async Task CancelQuote_WithCommand_ShouldCancelQuote()
	{
		// Given
		Guid customerId = Guid.NewGuid();
		var quoteWriteRepository = new DummyEventStoreRepository<Quote>();

		_userInfoRequester.RequestUserInfoAsync()
			.Returns(Task.FromResult(new UserInfo()
			{
				UserId = Guid.NewGuid().ToString(),
				CustomerId = customerId
			}));

		var openCommand = OpenQuote.Create(_currency);
		var openCommandHandler = new OpenQuoteHandler(_userInfoRequester, 
			quoteWriteRepository, _customerOpenQuoteChecker);
		await openCommandHandler.HandleAsync(openCommand, CancellationToken.None);

		var quote = quoteWriteRepository.AggregateStream.First().Aggregate;
		var cancelCommand = CancelQuote.Create(quote.Id);
		var cancelCommandHandler = new CancelQuoteHandler(quoteWriteRepository);

		// When
		await cancelCommandHandler.HandleAsync(cancelCommand, CancellationToken.None);

		// Then		
		Assert.Equal(QuoteStatus.Cancelled, quote.Status);

	}

	private Currency _currency = Currency.OfCode(Currency.USDollar.Code);
	private ICustomerOpenQuoteChecker _customerOpenQuoteChecker = Substitute.For<ICustomerOpenQuoteChecker>();
	private IUserInfoRequester _userInfoRequester = Substitute.For<IUserInfoRequester>();
}