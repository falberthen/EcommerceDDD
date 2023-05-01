using EcommerceDDD.IdentityServer.API.Controllers.Requests;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;

namespace EcommerceDDD.IdentityServer.Services;

public interface IIdentityManager
{
    Task<TokenResponse> AuthUserByCredentials(LoginRequest request);
    Task<IdentityResult> RegisterNewUser(RegisterUserRequest request);
}
