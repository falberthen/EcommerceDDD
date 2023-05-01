namespace EcommerceDDD.Products.Tests;

public class ProductCreationTests
{
    [Fact]
    public void Create_WithProductData_ReturnsProduct()
    {
        // Given
        var productPrice = Money.Of(100, Currency.USDollar.Code);
        var productName = "Product XYZ";
        var productCategory = "electronics";
        var productDescription = "USB 3.0 and USB 2.0 Compatibility Fast data transfers Improve PC Performance High Capacity";
        var productImageUrl = "https://fakestoreapi.com/img/61IBBVJvSDL._AC_SY879_.jpg";
        var productData = new ProductData(
            productName, 
            productCategory,
            productDescription,
            productImageUrl,
            productPrice);

        // When
        var product = Product.Create(productData);

        // Then
        Assert.NotNull(product);
        product.Id.Should().NotBe(null);
        product.Name.Should().Be(productName);
        product.Category.Should().Be(productCategory);
        product.Description.Should().Be(productDescription);
        product.ImageUrl.Should().Be(productImageUrl);
        product.UnitPrice.Should().Be(productPrice);
    }
}