namespace EcommerceDDD.Core.Infrastructure.Identity;

/// <summary>
/// Ownership checks for customer-scoped resources
/// (OWASP API1 - Broken Object Level Authorization).
/// </summary>
public static class CustomerOwnershipExtensions
{
	/// <summary>
	/// Fails with <see cref="ForbiddenError"/> when the resource belongs to a different customer.
	/// </summary>
	/// <param name="resourceCustomerId">The customer the resource belongs to.</param>
	public static Result EnsureCurrentCustomerOwns(
		this IUserInfoRequester userInfoRequester, Guid resourceCustomerId)
	{
		var userInfo = userInfoRequester.GetCurrentUser();

		if (userInfo.CustomerId != resourceCustomerId)
			return Result.Fail(
				new ForbiddenError("Resource does not belong to the current customer."));

		return Result.Ok();
	}
}
