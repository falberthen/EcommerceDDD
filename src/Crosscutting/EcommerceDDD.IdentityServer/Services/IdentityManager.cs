namespace EcommerceDDD.IdentityServer.Services;

public class IdentityManager(
	ITokenRequester tokenRequester,
	UserManager<ApplicationUser> userManager,
	IdentityApplicationDbContext dbContext,
	IEmailSender<ApplicationUser> emailSender,
	IOptions<EmailSettings> emailSettings,
	ILogger<IdentityManager> logger) : IIdentityManager
{
	private readonly ITokenRequester _tokenRequester = tokenRequester
		?? throw new ArgumentNullException(nameof(tokenRequester));
	private readonly UserManager<ApplicationUser> _userManager = userManager
		?? throw new ArgumentNullException(nameof(userManager));
	private readonly IdentityApplicationDbContext _dbContext = dbContext
		?? throw new ArgumentNullException(nameof(dbContext));
	private readonly IEmailSender<ApplicationUser> _emailSender = emailSender
		?? throw new ArgumentNullException(nameof(emailSender));
	private readonly EmailSettings _emailSettings = emailSettings?.Value
		?? throw new ArgumentNullException(nameof(emailSettings));
	private readonly ILogger<IdentityManager> _logger = logger
		?? throw new ArgumentNullException(nameof(logger));

	public async Task<LoginResult?> AuthUserByCredentials(LoginRequest request)
	{
		var response = await _tokenRequester.GetUserTokenFromCredentialsAsync(
			request.Email, request.Password
		);

		if (response?.HttpStatusCode == HttpStatusCode.BadRequest)
			return null;

		return new LoginResult()
		{
			AccessToken = response?.AccessToken,
			ErrorDescription = response?.ErrorDescription,
			IdentityToken = response?.IdentityToken,
			RefreshToken = response?.RefreshToken,
		};
	}

	public async Task<bool> IsAwaitingEmailConfirmation(LoginRequest request)
	{
		// Only tells the caller about the pending confirmation when the password is right.
		var user = await _userManager.FindByEmailAsync(request.Email);
		return user is not null
			&& !user.EmailConfirmed
			&& await _userManager.CheckPasswordAsync(user, request.Password);
	}

	/// <summary>
	/// Creates the user under the id the caller chose (the customer id), so the call is idempotent.
	/// </summary>
	public async Task<UserRegistration> RegisterNewUser(RegisterUserRequest request)
	{
		var userId = request.CustomerId.ToString();

		var existing = await _userManager.FindByIdAsync(userId);
		if (existing is not null)
		{
			if (!await IsSameRegistration(existing, request))
				return UserRegistration.EmailTaken();

			// Someone registering again most likely never found the first e-mail.
			await TrySendConfirmationLinkAsync(existing);
			return UserRegistration.Registered(existing.Id);
		}

		if (await _userManager.FindByEmailAsync(request.Email) is not null)
			return UserRegistration.EmailTaken();

		var user = new ApplicationUser
		{
			Id = userId,
			UserName = request.Email,
			Email = request.Email,
			EmailConfirmed = false,
		};

		// User, role and claims are a single unit
		await using (var transaction = await _dbContext.Database.BeginTransactionAsync())
		{
			var result = await _userManager.CreateAsync(user, request.Password);
			if (!result.Succeeded)
			{
				// Lost a race against a concurrent registration for the same e-mail.
				if (result.Errors.Any(e => e.Code is nameof(IdentityErrorDescriber.DuplicateUserName)
					or nameof(IdentityErrorDescriber.DuplicateEmail)))
					return UserRegistration.EmailTaken();

				return UserRegistration.Rejected(result.Errors.Select(e => e.Description));
			}

			EnsureSucceeded(await _userManager.AddToRoleAsync(user, Roles.Customer));
			EnsureSucceeded(await _userManager.AddClaimsAsync(user,
				[
					new Claim(JwtClaimTypes.Subject, user.Id),
					new Claim(JwtClaimTypes.Name, user.UserName),
					new Claim(JwtClaimTypes.Email, user.Email),
					new Claim(JwtClaimTypes.Role, Roles.Customer),
					new Claim(IdentityConfiguration.CustomerIdClaimType, request.CustomerId.ToString())
				]));

			await transaction.CommitAsync();
		}

		// Sent after the commit and never allowed to fail the registration. 
		// A lost e-mail is recovered by requesting a new confirmation link.
		await TrySendConfirmationLinkAsync(user);

		return UserRegistration.Registered(user.Id);
	}

	public async Task<bool> ConfirmEmail(ConfirmEmailRequest request)
	{
		var user = await _userManager.FindByIdAsync(request.UserId);
		if (user is null)
			return false;

		if (user.EmailConfirmed)
			return true;

		var token = DecodeToken(request.Token);
		if (token is null)
			return false;

		var result = await _userManager.ConfirmEmailAsync(user, token);
		return result.Succeeded;
	}

	public async Task ResendConfirmationLink(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user is null || user.EmailConfirmed)
			return;

		await TrySendConfirmationLinkAsync(user);
	}

	public async Task SendPasswordResetLink(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user is null)
			return;

		var token = await _userManager.GeneratePasswordResetTokenAsync(user);
		var link = BuildSpaLink("reset-password", user.Id, token);

		try
		{
			await _emailSender.SendPasswordResetLinkAsync(user, user.Email!, link);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to send the password reset link to user {UserId}.", user.Id);
		}
	}

	/// <summary>
	/// Resetting through the e-mailed link proves ownership of the address, so it also confirms it.
	/// This is what frees an address that was registered by someone else or with a forgotten password.
	/// </summary>
	public async Task<IdentityResult> ResetPassword(ResetPasswordRequest request)
	{
		var user = await _userManager.FindByIdAsync(request.UserId);
		var token = DecodeToken(request.Token);
		if (user is null || token is null)
			return IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());

		await using var transaction = await _dbContext.Database.BeginTransactionAsync();

		var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
		if (!result.Succeeded)
			return result;

		if (!user.EmailConfirmed)
		{
			user.EmailConfirmed = true;
			result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
				return result;
		}

		await transaction.CommitAsync();
		return result;
	}

	// A replay (e.g. an HTTP retry after a lost response) only happens before the e-mail is confirmed;
	// once it is, the account is settled and any new registration for it is a conflict.
	private async Task<bool> IsSameRegistration(ApplicationUser user, RegisterUserRequest request) =>
		!user.EmailConfirmed
		&& string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase)
		&& await _userManager.CheckPasswordAsync(user, request.Password);

	private async Task TrySendConfirmationLinkAsync(ApplicationUser user)
	{
		try
		{
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var link = BuildSpaLink("confirm-email", user.Id, token);
			await _emailSender.SendConfirmationLinkAsync(user, user.Email!, link);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to send the confirmation link to user {UserId}.", user.Id);
		}
	}

	private string BuildSpaLink(string path, string userId, string token)
	{
		// Identity tokens carry '+', '/' and '=', so they travel base64url-encoded.
		var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
		return $"{_emailSettings.SpaBaseUrl.TrimEnd('/')}/{path}" +
			$"?userId={Uri.EscapeDataString(userId)}&token={encodedToken}";
	}

	private static string? DecodeToken(string encodedToken)
	{
		try
		{
			return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
		}
		catch (FormatException)
		{
			return null;
		}
	}

	private static void EnsureSucceeded(IdentityResult result)
	{
		if (!result.Succeeded)
			throw new InvalidOperationException(result.Errors.First().Description);
	}
}
