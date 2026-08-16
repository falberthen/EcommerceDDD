namespace EcommerceDDD.Core.Infrastructure.Identity;

public class UserInfoRequester(IHttpContextAccessor httpContextAccessor) : IUserInfoRequester
{
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor
		?? throw new ArgumentNullException(nameof(httpContextAccessor));

	public UserInfo GetCurrentUser()
	{
		ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
		if (user?.Identity?.IsAuthenticated != true)
			throw new UnauthorizedAccessException("User is not authenticated.");

		var userId = user.FindFirstValue(JwtClaimTypes.Subject)
			?? user.FindFirstValue(ClaimTypes.NameIdentifier);
		var email = user.FindFirstValue(ClaimTypes.Email);
		var role = user.FindFirstValue(ClaimTypes.Role);
		var customerIdClaim = user.FindFirstValue(CustomClaimTypes.CustomerId);
		if (!Guid.TryParse(customerIdClaim, out var customerId))
			throw new InvalidOperationException(
				$"The '{CustomClaimTypes.CustomerId}' claim is missing or malformed.");

		return new UserInfo
		{
			UserId = userId ?? string.Empty,
			Email = email ?? string.Empty,
			Role = role ?? string.Empty,
			CustomerId = customerId
		};
	}
}

public record UserInfo
{
	public string UserId { get; init; } = default!;
	public string Email { get; init; } = default!;
	public string Role { get; init; } = default!;
	public Guid CustomerId { get; init; } = default!;
}