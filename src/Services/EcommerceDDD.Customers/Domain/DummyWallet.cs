using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Customers.Domain;

public class DummyWallet : ValueObject<DummyWallet>
{
    /// <summary>
    /// A very naive implementation, without Card collection or currency
    /// The only intent is to trigger saga compensation when credit limit is reached
    /// </summary>
    public decimal AvailableCreditLimit { get; private set; }

    public static DummyWallet CreateNew(decimal availableCreditLimit)
    {
        if(availableCreditLimit <= 0) 
            throw new DomainException("The available credit limit must be greater than zero.");

        return new DummyWallet(availableCreditLimit);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AvailableCreditLimit;
    }

    private DummyWallet(decimal availableCreditAmount)
    {
        AvailableCreditLimit = availableCreditAmount;        
    }
}