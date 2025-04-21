namespace EcommerceDDD.ProductCatalog.Tests;

public class ProductsControllerTests
{
    public ProductsControllerTests()
    {
        _productsController = new ProductsController(
            _commandBus,
            _queryBus);
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

		_queryBus.SendAsync(Arg.Any<GetProducts>(), CancellationToken.None)
			.Returns(expectedData);

		var request = new GetProductsRequest
		{
			CurrencyCode = Currency.USDollar.Symbol,
			ProductIds = new[] { productId }
		};

		// When
		var result = await _productsController.ListProducts(request, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(result);
		var apiResponse = Assert.IsType<ApiResponse<IList<ProductViewModel>>>(okResult.Value);
		Assert.IsAssignableFrom<IList<ProductViewModel>>(apiResponse.Data);
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

        _queryBus.SendAsync(Arg.Any<GetProducts>(), CancellationToken.None)
            .Returns(expectedData);

        var request = new GetProductsRequest()
        {
            CurrencyCode = Currency.USDollar.Symbol,
            ProductIds = [productId]
        };

        // When
        var response = await _productsController.ListProducts(request, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		var apiResponse = Assert.IsType<ApiResponse<IList<ProductViewModel>>>(okResult.Value);
		Assert.IsAssignableFrom<IList<ProductViewModel>>(apiResponse.Data);
	}

	private const int _productQuantity = 1;
	private const string _productName = "Product XYZ";
	private decimal _productPrice = 100;
	private Currency _currency = Currency.OfCode(Currency.USDollar.Code);
	private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IQueryBus _queryBus = Substitute.For<IQueryBus>();
    private ProductsController _productsController;
}