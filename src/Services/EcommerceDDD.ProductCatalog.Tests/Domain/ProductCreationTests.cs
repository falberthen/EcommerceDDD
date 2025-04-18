namespace EcommerceDDD.ProductCatalog.Tests;

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
		Assert.NotNull(product.Id);
		Assert.Equal(product.Name, productName);
		Assert.Equal(product.Category, productCategory);
		Assert.Equal(product.Description, productDescription);
		Assert.Equal(product.ImageUrl, productImageUrl);
		Assert.Equal(product.UnitPrice, productPrice);
    }
}