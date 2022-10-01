using IdentityModel.Client;

namespace EcommerceDDD.Core.Infrastructure.Identity;

public interface ITokenRequester
{
    Task<TokenResponse> GetApplicationToken(TokenIssuerSettings settings);
    Task<TokenResponse> GetUserToken(TokenIssuerSettings settings, string userName, string password);
}