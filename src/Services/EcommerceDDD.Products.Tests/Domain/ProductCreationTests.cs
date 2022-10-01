using EcommerceDDD.Products.Domain;

namespace EcommerceDDD.Products.Tests;

public class ProductCreationTests
{
    [Fact]
    public void Create_WithProductData_ReturnsProduct()
    {
        // Given
        var productPrice = Money.Of(100, Currency.USDollar.Code);
        var productName = "Product XYZ";
        var productData = new ProductData(productName, productPrice);

        // When
        var product = Product.Create(productData);

        // Then
        Assert.NotNull(product);
        product.Id.Should().NotBe(null);
        product.Name.Should().Be(productName);
        product.UnitPrice.Should().Be(productPrice);
    }
}