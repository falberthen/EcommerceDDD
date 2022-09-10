using EcommerceDDD.IntegrationServices.Customers.Responses;

namespace EcommerceDDD.IntegrationServices.Customers;

public interface ICustomersService
{
    Task<AvailableCreditLimitModel> RequestAvailableCreditLimit(string apiGatewayUrl, Guid customerId);
}
