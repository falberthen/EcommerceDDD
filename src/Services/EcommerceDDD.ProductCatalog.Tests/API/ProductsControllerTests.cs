namespace EcommerceDDD.ProductCatalog.Tests;

public class ProductsControllerTests
{
    public ProductsControllerTests()
    {
        _productsController = new ProductsController(_bus);
    }

	[Fact]
	public async Task ListProduct_WithGetProductsRequest_ShouldReturnListOfProductViewModel()
	{
		// Given
		var productId = Guid.NewGuid();
		var expectedData = new List<ProductViewModel>
		{
			new ProductViewModel(
				Guid.NewGuid(),
				_productName,
				string.Empty,
				string.Empty,
				string.Empty,
				_productPrice,
				_currency.Symbol.ToString(),
				100,
				_productQuantity)
		};

		_bus.InvokeAsync<Result<IReadOnlyList<ProductViewModel>>>(Arg.Any<GetProducts>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
			.Returns(Result.Ok<IReadOnlyList<ProductViewModel>>(expectedData));

		var request = new GetProductsRequest
		{
			CurrencyCode = Currency.USDollar.Symbol,
			ProductIds = new[] { productId }
		};

		// When
		var result = await _productsController.ListProducts(request, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(result);
		Assert.IsAssignableFrom<IList<ProductViewModel>>(okResult.Value);
	}

	[Fact]
    public async Task CheckStockAvailability_WithProductStockAvailabilityRequest_ShouldReturnProductInStockViewModel()
    {
        // Given
        var productId = Guid.NewGuid();
        var expectedData = new List<ProductViewModel>
        {
			new ProductViewModel(
				Guid.NewGuid(),
				_productName,
				string.Empty,
				string.Empty,
				string.Empty,
				_productPrice,
				_currency.Symbol.ToString(),
				100,
				_productQuantity)
		};

        _bus.InvokeAsync<Result<IReadOnlyList<ProductViewModel>>>(Arg.Any<GetProducts>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
            .Returns(Result.Ok<IReadOnlyList<ProductViewModel>>(expectedData));

        var request = new GetProductsRequest()
        {
            CurrencyCode = Currency.USDollar.Symbol,
            ProductIds = [productId]
        };

        // When
        var response = await _productsController.ListProducts(request, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		Assert.IsAssignableFrom<IList<ProductViewModel>>(okResult.Value);
	}

	private const int _productQuantity = 1;
	private const string _productName = "Product XYZ";
	private decimal _productPrice = 100;
	private Currency _currency = Currency.OfCode(Currency.USDollar.Code);
	private IMessageBus _bus = Substitute.For<IMessageBus>();
    private ProductsController _productsController;
}
