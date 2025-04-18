namespace EcommerceDDD.Core.Infrastructure.Identity;

public class UserInfoRequester(IHttpContextAccessor httpContextAccessor) : IUserInfoRequester
{
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor
		?? throw new ArgumentNullException(nameof(httpContextAccessor));

	public Task<UserInfo?> RequestUserInfoAsync()
	{
		ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
		if (user is null || !user.Identity?.IsAuthenticated == true)
			throw new UnauthorizedAccessException("User is not authenticated.");

		var userId = user.FindFirstValue(JwtClaimTypes.Subject)
			?? user.FindFirstValue(ClaimTypes.NameIdentifier);
		var email = user.FindFirstValue(ClaimTypes.Email);
		var role = user.FindFirstValue(ClaimTypes.Role);
		var customerId = user.FindFirstValue("CustomerId")
			?? throw new ArgumentNullException();

		var userInfo = new UserInfo
		{
			UserId = userId ?? string.Empty,
			Email = email ?? string.Empty,
			Role = role ?? string.Empty,
			CustomerId = new Guid(customerId)
		};

		return Task.FromResult<UserInfo?>(userInfo);
	}
}

public record UserInfo
{
	public string UserId { get; set; } = default!;
	public string Email { get; set; } = default!;
	public string Role { get; set; } = default!;
	public Guid CustomerId { get; set; } = default!;
}