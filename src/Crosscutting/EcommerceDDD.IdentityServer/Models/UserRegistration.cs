namespace EcommerceDDD.IdentityServer.Models;

public enum RegistrationStatus
{
	Registered,
	EmailTaken,
	Rejected
}

public record UserRegistration(RegistrationStatus Status, string? UserId, IReadOnlyList<string> Errors)
{
	public static UserRegistration Registered(string userId) =>
		new(RegistrationStatus.Registered, userId, []);

	public static UserRegistration EmailTaken() =>
		new(RegistrationStatus.EmailTaken, null, []);

	public static UserRegistration Rejected(IEnumerable<string> errors) =>
		new(RegistrationStatus.Rejected, null, errors.ToList());
}
