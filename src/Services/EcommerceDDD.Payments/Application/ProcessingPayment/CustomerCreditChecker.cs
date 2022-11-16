using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.Infrastructure.Integration;

namespace EcommerceDDD.Payments.Application.ProcessingPayment;

public class CustomerCreditChecker : ICustomerCreditChecker
{
    private readonly IIntegrationHttpService _integrationHttpService;

    public CustomerCreditChecker(IIntegrationHttpService integrationHttpService)
    {
        _integrationHttpService = integrationHttpService;
    }

    public async Task<bool> IsCreditEnough(CustomerId customerId, Money totalAmount)
    {
        // Checking customer's credit        
        var response = await _integrationHttpService
            .GetAsync<CreditLimitModel>(
                $"api/customers/{customerId.Value}/credit");

        if (response is null || !response!.Success)
            throw new ApplicationLogicException($"An error ocurred trying to obtain the credit limit for customer {customerId.Value}");

        var customerCreditLimit = response.Data
            ?? throw new RecordNotFoundException("No data was provided for customer credit limit.");

        // Simply comparing with the customer credit limit
        return totalAmount.Amount < customerCreditLimit.CreditLimit;
    }
}
