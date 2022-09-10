using Moq;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Customers.Tests.Domain;

public class CustomerCreationTests
{
    [Fact]
    public async Task Create_WithUniqueEmail_ReturnsCustomer()
    {
        // Given
        _checker.Setup(p => p.IsUserUnique(It.IsAny<string>()))
            .Returns(true);

        // When
        var customer = Customer.CreateNew(_email, _name, _address, _availableCreditLimit,
            _checker.Object);

        // Then
        Assert.NotNull(customer);
        customer.Id.Value.Should().NotBe(Guid.Empty);
        customer.Email.Should().Be(_email);
        customer.Name.Should().Be(_name);
        customer.Address.Should().Be(_address);
        customer.Wallet.Should().Be(DummyWallet.CreateNew(_availableCreditLimit));
    }

    [Fact]
    public async Task Create_WithExistingEmail_ThrowsException()
    {
        // Given
        _checker.Setup(p => p.IsUserUnique(It.IsAny<string>()))
            .Returns(false);

        // When
        Func<Customer> action = () =>
            Customer.CreateNew(_email, _name, _address, _availableCreditLimit, 
            _checker.Object);

        // Then
        action.Should().Throw<DomainException>();
    }

    private const string _email = "email@test.com";
    private const string _name = "UserTest";
    private const string _address = "Rue XYZ";
    private const decimal _availableCreditLimit = 1000;
    private Mock<ICustomerUniquenessChecker> _checker = new();
}