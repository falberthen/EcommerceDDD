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
				productId,
				"Product",
				"Category",
				"Description",
				"ImageUrl",
				"10",
				Currency.USDollar.Symbol,
				100)
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
                productId,
                "Product",
                "Category",
                "Description",
                "ImageUrl",
                "10",
                Currency.USDollar.Symbol,
                100)
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

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IQueryBus _queryBus = Substitute.For<IQueryBus>();
    private ProductsController _productsController;
}