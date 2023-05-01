namespace EcommerceDDD.IdentityServer.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IIdentityManager _identityManager;
    
    public AccountsController(
        IIdentityManager identityManager)
    {
        _identityManager = identityManager;
    }

    [HttpPost, Route("login")]
    public async Task<IActionResult> UserLogin(LoginRequest request)
    {
        var response = await _identityManager
            .AuthUserByCredentials(request);

        return Ok(response);
    }

    [HttpPost, Route("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        IdentityResult result = default!;

        try
        {
            result = await _identityManager
                .RegisterNewUser(request);
        }
        catch (Exception)
        {
            return BadRequest(result);            
        }
        
        return Ok(new
        {
            data = result,
            success = result.Succeeded
        });
    }

    private BadRequestObjectResult BadRequest(IdentityResult result) => 
        BadRequest(new
        {
            success = result.Succeeded,
            message = result.Errors.First().Description
        });
}
