namespace EcommerceDDD.CustomerManagement.Application.GettingStoreCredit;

public record class GetStoreCredit : IQuery<StoreCreditModel>
{
	public CustomerId CustomerId { get; private set; }

	public static GetStoreCredit Create(CustomerId customerId)
	{
		if (customerId is null)
			throw new ArgumentNullException(nameof(customerId));

		return new GetStoreCredit(customerId);
	}

	private GetStoreCredit(CustomerId customerId) => CustomerId = customerId;
}