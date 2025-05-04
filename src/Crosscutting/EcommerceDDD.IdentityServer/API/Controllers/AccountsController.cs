namespace EcommerceDDD.IdentityServer.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/v{version:apiVersion}/accounts")]
public class AccountsController(IIdentityManager identityManager) : CustomControllerBase
{
    private readonly IIdentityManager _identityManager = identityManager
		?? throw new ArgumentNullException(nameof(identityManager));

	[HttpPost, Route("login")]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
	public async Task<IActionResult> UserLogin(LoginRequest request)
    {
        try
        {
			LoginResult result = await _identityManager
                .AuthUserByCredentials(request);

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequestActionResult(e.Message);
        }
    }

    [HttpPost, Route("register")]
	[MapToApiVersion(ApiVersions.V2)]
	[ProducesResponseType(typeof(UserRegisteredResult), StatusCodes.Status200OK)]
	public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        try
        {
			UserRegisteredResult result = await _identityManager
                .RegisterNewUser(request);

            return Ok(new
            {
                data = result,
                success = result.Succeeded
            });
        }
        catch (Exception e)
        {
            return BadRequestActionResult(e.Message);            
        }
    }
}
