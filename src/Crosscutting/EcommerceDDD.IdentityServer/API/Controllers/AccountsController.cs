using IdentityModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using EcommerceDDD.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using EcommerceDDD.IdentityServer.Models;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.IdentityServer.API.Controllers.Requests;

namespace EcommerceDDD.IdentityServer.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly ITokenRequester _tokenRequester;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenIssuerSettings _issuerSettings;

    public AccountsController(
        IOptions<TokenIssuerSettings> issuerSettings,
        ITokenRequester tokenRequester,
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _tokenRequester = tokenRequester;
        _issuerSettings = issuerSettings.Value;
    }

    [HttpPost, Route("login")]
    public async Task<IActionResult> UserLogin(LoginRequest request)
    {
        var response = await _tokenRequester.GetUserToken(
            _issuerSettings,
            request.Email,
            request.Password);

        return Ok(response);
    }

    [HttpPost, Route("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
        };

        var result = await _userManager
            .CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(result);

        result = await _userManager.AddClaimsAsync(user,
            new Claim[]
            {
                new Claim(JwtClaimTypes.Email, request.Email),
            });

        if (!result.Succeeded)        
            return BadRequest(result);
        
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
