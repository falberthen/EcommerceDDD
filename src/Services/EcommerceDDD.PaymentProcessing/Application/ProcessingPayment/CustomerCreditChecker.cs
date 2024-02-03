namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

public class CustomerCreditChecker : ICustomerCreditChecker
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IConfiguration _configuration;

    public CustomerCreditChecker(
        IIntegrationHttpService integrationHttpService,
        IConfiguration configuration)
    {
        _integrationHttpService = integrationHttpService;
        _configuration = configuration;
    }

    public async Task<bool> IsCreditEnough(CustomerId customerId, Money totalAmount)
    {
        // Checking customer's credit
        var apiRoute = _configuration["ApiRoutes:CustomerManagement"];
        var response = await _integrationHttpService
            .GetAsync<CreditLimitModel>(
                $"{apiRoute}/{customerId.Value}/check-credit");

        if (response?.Success == false)
            throw new ApplicationLogicException($"An error ocurred trying to obtain the credit limit for customer {customerId.Value}");

        var customerCreditLimit = response.Data
            ?? throw new RecordNotFoundException("No data was provided for customer credit limit.");

        // Simply comparing with the customer credit limit
        return totalAmount.Amount < customerCreditLimit.CreditLimit;
    }
}

public record class CreditLimitModel(Guid CustomerId, decimal CreditLimit);