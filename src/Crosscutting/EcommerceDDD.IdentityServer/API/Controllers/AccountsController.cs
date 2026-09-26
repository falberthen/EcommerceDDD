namespace EcommerceDDD.IdentityServer.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/accounts")]
public class AccountsController(IIdentityManager identityManager) : CustomControllerBase
{
	private readonly IIdentityManager _identityManager = identityManager
		?? throw new ArgumentNullException(nameof(identityManager));

	[HttpPost("login")]
	[AllowAnonymous]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
	public async Task<IActionResult> UserLogin([FromBody] LoginRequest request)
	{
		if (request is null)
			return this.BadRequestProblem("Request body is required.", "Invalid request");

		try
		{
			if (await _identityManager.IsAwaitingEmailConfirmation(request))
				return this.ForbiddenProblem(
					"Your e-mail is not confirmed yet. Check your inbox or request a new confirmation link.",
					"E-mail not confirmed");

			var result = await _identityManager.AuthUserByCredentials(request);
			if (result is null)
				return this.UnauthorizedProblem("Invalid credentials.", "Login failed");

			return Ok(result);
		}
		catch (Exception)
		{
			return this.InternalServerErrorProblem(
				"Unexpected error while processing login.",
				"Internal server error");
		}
	}

	/// <summary>
	/// Creates the user for a customer. Machine-to-machine only: the customer registration flow in CustomerManagement is the single entry point for new accounts.
	/// </summary>
	[HttpPost("register")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.M2MAccess)]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(typeof(UserRegisteredResult), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
	{
		if (request is null)
			return this.BadRequestProblem("Request body is required.", "Invalid request");

		var registration = await _identityManager.RegisterNewUser(request);

		return registration.Status switch
		{
			RegistrationStatus.Registered => Ok(new UserRegisteredResult
			{
				UserId = registration.UserId!,
				Succeeded = true
			}),
			RegistrationStatus.EmailTaken => this.ConflictProblem(
				"An account with this e-mail already exists.",
				"Registration failed"),
			_ => this.ValidationProblemResponse(
				detail: registration.Errors.FirstOrDefault() ?? "Registration could not be completed.",
				errors: new Dictionary<string, string[]> { ["registration"] = [.. registration.Errors] },
				title: "Validation failed")
		};
	}

	[HttpPost("confirm-email")]
	[AllowAnonymous]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
	{
		var confirmed = await _identityManager.ConfirmEmail(request);
		return confirmed
			? Ok()
			: this.BadRequestProblem(
				"This confirmation link is invalid or has expired. Request a new one.",
				"Confirmation failed");
	}

	/// <summary>
	/// Always succeeds, so the response does not reveal whether the e-mail is registered.
	/// </summary>
	[HttpPost("resend-confirmation")]
	[AllowAnonymous]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(StatusCodes.Status202Accepted)]
	public async Task<IActionResult> ResendConfirmation([FromBody] EmailRequest request)
	{
		await _identityManager.ResendConfirmationLink(request.Email);
		return Accepted();
	}

	/// <summary>
	/// Always succeeds, so the response does not reveal whether the e-mail is registered.
	/// </summary>
	[HttpPost("forgot-password")]
	[AllowAnonymous]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(StatusCodes.Status202Accepted)]
	public async Task<IActionResult> ForgotPassword([FromBody] EmailRequest request)
	{
		await _identityManager.SendPasswordResetLink(request.Email);
		return Accepted();
	}

	[HttpPost("reset-password")]
	[AllowAnonymous]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
	{
		var result = await _identityManager.ResetPassword(request);
		if (result.Succeeded)
			return Ok();

		var errors = result.Errors.Select(e => e.Description).ToArray();
		return this.ValidationProblemResponse(
			detail: errors.FirstOrDefault() ?? "The password could not be reset.",
			errors: new Dictionary<string, string[]> { ["password"] = errors },
			title: "Validation failed");
	}
}
