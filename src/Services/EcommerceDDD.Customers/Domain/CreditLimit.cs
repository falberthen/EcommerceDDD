using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Customers.Domain;

public class CreditLimit : ValueObject<CreditLimit>
{
    public decimal Amount { get; private set; }

    public static CreditLimit Create(decimal creditLimit)
    {
        if(creditLimit <= 0) 
            throw new BusinessRuleException("The customer credit limit must be greater than zero.");

        return new CreditLimit(creditLimit);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }

    private CreditLimit(decimal creditLimit)
    {
        Amount = creditLimit;        
    }
}