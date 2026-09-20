namespace EcommerceDDD.CustomerManagement.Domain;

public class StoreCredit : ValueObject<StoreCredit>
{
    public decimal Amount { get; private set; }

    public static StoreCredit Create(decimal storeCredit)
    {
        if(storeCredit <= 0) 
            throw new DomainException("The customer store credit must be greater than zero.");

        return new StoreCredit(storeCredit);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }

    private StoreCredit(decimal storeCredit) => Amount = storeCredit;
}