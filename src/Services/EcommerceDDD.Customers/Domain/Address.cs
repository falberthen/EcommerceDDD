namespace EcommerceDDD.Customers.Domain;

public class Address : ValueObject<Address>
{
    public string StreetAddress { get; private set; }

    public static Address FromStreetAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new BusinessRuleException("Address cannot be null or whitespace.");

        return new Address(address);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StreetAddress;
    }

    private Address(string address)
    {
        StreetAddress = address;        
    }
}