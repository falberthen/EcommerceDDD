namespace EcommerceDDD.IdentityServer.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/v{version:apiVersion}/accounts")]
public class AccountsController(IIdentityManager identityManager) : CustomControllerBase
{
	private readonly IIdentityManager _identityManager = identityManager
		?? throw new ArgumentNullException(nameof(identityManager));

	[HttpPost("login")]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
	public async Task<IActionResult> UserLogin([FromBody] LoginRequest request)
	{
		if (request is null)		
			return this.BadRequestProblem("Request body is required.", "Invalid request");		

		try
		{
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

	[HttpPost("register")]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(typeof(UserRegisteredResult), StatusCodes.Status200OK)]
	public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
	{
		if (request is null)
			return this.BadRequestProblem("Request body is required.", "Invalid request");

		try
		{
			var result = await _identityManager.RegisterNewUser(request);

			if (!result.Succeeded)
			{
				var errors = new Dictionary<string, string[]>
				{
					["registration"] = new[] { "Registration could not be completed." }
				};

				return this.ValidationProblemResponse(
					detail: "Registration could not be completed.",
					errors: errors,
					title: "Validation failed");
			}

			return Ok(new
			{
				data = result,
				success = result.Succeeded
			});
		}
		catch (InvalidOperationException ex)
		{
			return this.BadRequestProblem(ex.Message, "Registration failed");
		}
		catch (Exception)
		{
			return this.InternalServerErrorProblem(
				"Unexpected error while processing registration.",
				"Internal server error");
		}
	}
}