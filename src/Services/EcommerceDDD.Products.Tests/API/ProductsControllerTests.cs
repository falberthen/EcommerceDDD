namespace EcommerceDDD.Products.Tests;

public class ProductsControllerTests
{    
    public ProductsControllerTests()
    {
        _productsController = new ProductsController(
            _commandBus.Object,
            _queryBus.Object);
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
                "$" )
        };

        _queryBus
            .Setup(m => m.Send(It.IsAny<GetProducts>()))
            .ReturnsAsync(expectedData);

        var request = new GetProductsRequest()
        {
            CurrencyCode = "$",
            ProductIds = new Guid[] { productId }
        };

        // When
        var response = await _productsController.ListProducts(request);

        // Then
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<IList<ProductViewModel>>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
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
                "$" )
        };

        _queryBus
            .Setup(m => m.Send(It.IsAny<GetProducts>()))
            .ReturnsAsync(expectedData);

        var request = new GetProductsRequest()
        {
            CurrencyCode = "$",
            ProductIds = new Guid[] { productId }
        };

        // When
        var response = await _productsController.ListProducts(request);

        // Then
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<IList<ProductViewModel>>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
    }

    private Mock<ICommandBus> _commandBus = new();
    private Mock<IQueryBus> _queryBus = new();
    private ProductsController _productsController;
}