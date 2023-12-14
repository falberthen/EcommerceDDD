using EcommerceDDD.Customers.Domain;
using EcommerceDDD.Customers.Api.Application.RegisteringCustomer;

namespace EcommerceDDD.Customers.Tests.Application;

public class RegisterCustomerHandlerTests
{
    public RegisterCustomerHandlerTests()
    {
        _options.Value
            .Returns(new TokenIssuerSettings() { Authority = "http://url" });

        _requester.PostAsync<IntegrationHttpResponse>(Arg.Any<string>(), Arg.Any<object>())!
           .Returns(Task.FromResult(new IntegrationHttpResponse() { Success = true }));
    }

    [Fact]
    public async Task Register_WithCommand_ShouldRegisterCustomer()
    {
        // Given
        _checker.IsUnique(Arg.Any<string>())
            .Returns(true);
        
        var confirmation = _password;
        var registerCommand = RegisterCustomer
            .Create(_email, _password, confirmation, _name, _streetAddress, _creditLimit);
        var commandHandler = new RegisterCustomerHandler(
            _requester,
            _checker, 
            _options,
            _dummyRepository);

        // When
        await commandHandler.Handle(registerCommand, CancellationToken.None);

        // Then
        var addedCustomer = _dummyRepository.AggregateStream.First().Aggregate;
        addedCustomer.Email.Should().Be(registerCommand.Email);
        addedCustomer.Name.Should().Be(registerCommand.Name);
        addedCustomer.ShippingAddress.Should().Be(Address.FromStreetAddress(_streetAddress));
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldThrownException()
    {
        // Given       
        _checker.IsUnique(Arg.Any<string>())
            .Returns(false);

        var confirmation = _password;
        var registerCommand = RegisterCustomer
            .Create(_email, _password, confirmation, _name, _streetAddress, _creditLimit);
        var commandHandler = new RegisterCustomerHandler(
            _requester,
            _checker,
            _options,
            _dummyRepository);

        // When
        Func<Task> action = async () =>
            await commandHandler.Handle(registerCommand, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<BusinessRuleException>();
    }

    public const string _email = "email@test.com";
    public const string _name = "UserTest";
    public const string _password = "p4ssw0rd";
    public const string _streetAddress = "Rue XYZ";
    public const decimal _creditLimit = 1000;
    private IHttpRequester _requester = Substitute.For<IHttpRequester>();
    private IEmailUniquenessChecker _checker = Substitute.For<IEmailUniquenessChecker>();
    private IOptions<TokenIssuerSettings> _options = Substitute.For<IOptions<TokenIssuerSettings>>();
    private DummyEventStoreRepository<Customer> _dummyRepository = new DummyEventStoreRepository<Customer>();
}