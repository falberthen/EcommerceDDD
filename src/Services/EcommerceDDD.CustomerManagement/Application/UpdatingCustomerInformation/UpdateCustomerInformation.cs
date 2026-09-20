namespace EcommerceDDD.CustomerManagement.Application.UpdatingCustomerInformation;

public record class UpdateCustomerInformation : ICommand
{
	public string Name { get; private set; }
	public string ShippingAddress { get; private set; }
	public decimal StoreCredit { get; private set; }

	public static UpdateCustomerInformation Create(
		string name,
		string shippingAddress,
		decimal storeCredit)
	{
		if (string.IsNullOrEmpty(name))
			throw new ArgumentNullException(nameof(name));
		if (string.IsNullOrEmpty(shippingAddress))
			throw new ArgumentNullException(nameof(shippingAddress));
		if (storeCredit <= 0)
			throw new ArgumentOutOfRangeException(nameof(storeCredit));

		return new UpdateCustomerInformation(
			name,
			shippingAddress,
			storeCredit);
	}

	private UpdateCustomerInformation(
		string name,
		string shippingAddress,
		decimal storeCredit)
	{
		Name = name;
		ShippingAddress = shippingAddress;
		StoreCredit = storeCredit;
	}
}
