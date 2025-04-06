using EcommerceDDD.Core.Infrastructure.Identity;

namespace EcommerceDDD.QuoteManagement.Tests.Application;

public class OpenQuoteHandlerTests
{
	[Fact]
	public async Task OpenQuote_WithCommand_ShouldCreateQuote()
	{
		// Given
		var customerId = CustomerId.Of(Guid.NewGuid());
		var quoteWriteRepository = new DummyEventStoreRepository<Quote>();

		_userInfoRequester.RequestUserInfoAsync()
			.Returns(Task.FromResult(new UserInfo()
			{
				UserId = Guid.NewGuid().ToString(),
				CustomerId = customerId.Value
			}));

		var currency = Currency.OfCode(Currency.USDollar.Code);
		var command = OpenQuote.Create(currency);
		var commandHandler = new OpenQuoteHandler(_userInfoRequester, 
			quoteWriteRepository, _customerOpenQuoteChecker);

		// When
		await commandHandler.Handle(command, CancellationToken.None);

		// Then
		quoteWriteRepository.AggregateStream.Should().HaveCount(1);
		var openQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
		openQuote.CustomerId.Should().Be(customerId);
		openQuote.Status.Should().Be(QuoteStatus.Open);
	}

	private ICustomerOpenQuoteChecker _customerOpenQuoteChecker = Substitute.For<ICustomerOpenQuoteChecker>();
	private IUserInfoRequester _userInfoRequester = Substitute.For<IUserInfoRequester>();
}