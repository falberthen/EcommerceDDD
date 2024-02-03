namespace EcommerceDDD.CustomerManagement.Api.Application.GettingCustomerDetails;

public class GetCustomerDetailsHandler : IQueryHandler<GetCustomerDetails, CustomerDetails>
{
    private readonly IQuerySession _querySession;

    public GetCustomerDetailsHandler(        
        IQuerySession querySession)
    {        
        _querySession = querySession;
    }

    public async Task<CustomerDetails> Handle(GetCustomerDetails query, 
        CancellationToken cancellationToken)
    {
        var customer = await _querySession.Query<CustomerDetails>()
            .FirstOrDefaultAsync(c => c.Id == query.CustomerId.Value)
            ?? throw new RecordNotFoundException($"Customer not found.");

        var details = new CustomerDetails();
        details.Id = customer.Id;
        details.Email = customer.Email;
        details.Name = customer.Name;
        details.ShippingAddress = customer.ShippingAddress;
        details.CreditLimit = customer.CreditLimit;

        return details;
    }
}
