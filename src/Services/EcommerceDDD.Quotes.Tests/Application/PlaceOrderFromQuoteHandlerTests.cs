using EcommerceDDD.Core.Testing;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Quotes.Domain.Commands;
using EcommerceDDD.Quotes.Application.Products;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Quotes.Application.Quotes.PlacingOrderFromQuote;
using EcommerceDDD.Quotes.Application.Quotes.AddingQuoteItem;
using EcommerceDDD.Quotes.Application.Quotes.OpeningQuote;

namespace EcommerceDDD.Quotes.Tests.Application;

public class PlaceOrderFromQuoteHandlerTests
{
    [Fact]
    public async Task PlaceOrder_WithCommand_ShouldConfirmQuote()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        var productId = ProductId.Of(Guid.NewGuid());
        const string productName = "Product XYZ";
        const int productQuantity = 1;
        var currency = Currency.OfCode(Currency.USDollar.Code);

        var responseProducts = new IntegrationHttpResponse<List<ProductViewModel>>()
        {
            Success = true,
            Data = new List<ProductViewModel>()
            {
                new ProductViewModel(
                    productId.Value,
                    productName,
                    productQuantity,
                    currency.Symbol
                )
            }
        };
        var responseOrder = new IntegrationHttpResponse() { Success = true };

        _integrationHttpService.Setup(p =>
            p.FilterAsync<List<ProductViewModel>>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(Task.FromResult(responseProducts));
        _integrationHttpService.Setup(p =>
            p.PostAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(Task.FromResult(responseOrder));
        _checker.Setup(p => p.CustomerHasOpenQuote(customerId))
            .Returns(Task.FromResult(true));

        var quote = Quote.Create(customerId);

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
        await quoteWriteRepository.AppendEventsAsync(quote);

        var quoteItemRequest = quote.Items.Select(q =>
            new QuoteItemRequest(q.ProductId.Value, q.Quantity))
            .ToList();

        var addQuoteItem = AddQuoteItem.Create(quote.Id, productId, productQuantity, currency);
        var addQuoteItemHandler = new AddQuoteItemHandler(_integrationHttpService.Object, quoteWriteRepository);
        await addQuoteItemHandler.Handle(addQuoteItem, CancellationToken.None);

        var placeOrderFromQuote = PlaceOrderFromQuote.Create(quote.Id, currency);
        var placeOrderFromQuoteHandler = new PlaceOrderFromQuoteHandler(_integrationHttpService.Object, quoteWriteRepository);

        // When
        await placeOrderFromQuoteHandler.Handle(placeOrderFromQuote, CancellationToken.None);

        // Then
        var storedQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        storedQuote.Status.Should().Be(QuoteStatus.Confirmed);
    }

    private Mock<ICustomerOpenQuoteChecker> _checker = new();
    private Mock<IIntegrationHttpService> _integrationHttpService = new();
}