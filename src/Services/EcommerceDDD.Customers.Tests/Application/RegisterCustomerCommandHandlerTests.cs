using Moq;
using EcommerceDDD.Core.Testing;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Customers.Api.Application.RegisteringCustomer;

namespace EcommerceDDD.Customers.Tests.Application;

public class RegisterCustomerCommandHandlerTests
{
    [Fact]
    public async Task Register_WithCommand_ShouldRegisterCustomer()
    {
        // Given
        _checker.Setup(p => p.IsUserUnique(It.IsAny<string>()))
            .Returns(true);
        
        var customerWriteRepository = new DummyEventStoreRepository<Customer>();

        var options = new Mock<IOptions<TokenIssuerSettings>>();
        options.Setup(p => p.Value)
            .Returns(new TokenIssuerSettings() { Authority = "http://url" });
        
        var requester = new Mock<IHttpRequester>();
        requester.Setup(p => p.PostAsync<HttpResponseMessage>(It.IsAny<string>(), It.IsAny<object>(), null))
            .Returns(Task.FromResult(new HttpResponseMessage()));

        string password, confirmation;
        password = confirmation = "p4ssw0rd";

        var command = new RegisterCustomer(_email, password, confirmation, _name, _address);
        var commandHandler = new RegisterCustomerHandler(
            requester.Object,
            _checker.Object, 
            options.Object,
            customerWriteRepository);

        // When
        await commandHandler.Handle(command, CancellationToken.None);

        // Then
        var addedCustomer = customerWriteRepository.AggregateStream.First().Aggregate;
        addedCustomer.Email.Should().Be(command.Email);
        addedCustomer.Name.Should().Be(command.Name);
        addedCustomer.Address.Should().Be(command.Address);
    }

    private const string _email = "email@test.com";
    private const string _name = "UserTest";
    private const string _address = "Rue XYZ";
    private Mock<ICustomerUniquenessChecker> _checker = new();
}