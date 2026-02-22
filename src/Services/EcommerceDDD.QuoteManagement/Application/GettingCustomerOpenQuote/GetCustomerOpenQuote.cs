namespace EcommerceDDD.QuoteManagement.Application.GettingCustomerOpenQuote;

public record class GetCustomerOpenQuote : IQuery<QuoteViewModel>
{
    public static GetCustomerOpenQuote Create()
    {        
        return new GetCustomerOpenQuote();
    }

    private GetCustomerOpenQuote() {}
}