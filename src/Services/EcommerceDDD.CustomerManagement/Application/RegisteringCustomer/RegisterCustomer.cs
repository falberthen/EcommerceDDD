namespace EcommerceDDD.CustomerManagement.Application.RegisteringCustomer;

public record class RegisterCustomer : ICommand
{
	public string Email { get; private set; }
	public string Password { get; private set; }
	public string PasswordConfirm { get; private set; }
	public string Name { get; private set; }
	public string ShippingAddress { get; private set; }
	public decimal StoreCredit { get; private set; }

	public static RegisterCustomer Create(
		string email,
		string password,
		string passwordConfirm,
		string name,
		string shippingAddress,
		decimal storeCredit)
	{
		if (string.IsNullOrEmpty(email))
			throw new ArgumentNullException(nameof(email));
		if (string.IsNullOrEmpty(password))
			throw new ArgumentNullException(nameof(password));
		if (string.IsNullOrEmpty(passwordConfirm))
			throw new ArgumentNullException(nameof(passwordConfirm));
		if (string.IsNullOrEmpty(name))
			throw new ArgumentNullException(nameof(name));
		if (string.IsNullOrEmpty(shippingAddress))
			throw new ArgumentNullException(nameof(shippingAddress));
		if (storeCredit <= 0)
			throw new ArgumentOutOfRangeException(nameof(storeCredit));

		return new RegisterCustomer(email,
			password,
			passwordConfirm,
			name,
			shippingAddress,
			storeCredit);
	}

	private RegisterCustomer(
		string email,
		string password,
		string passwordConfirm,
		string name,
		string shippingAddress,
		decimal storeCredit)
	{
		Email = email;
		Password = password;
		PasswordConfirm = passwordConfirm;
		Name = name;
		ShippingAddress = shippingAddress;
		StoreCredit = storeCredit;
	}
}