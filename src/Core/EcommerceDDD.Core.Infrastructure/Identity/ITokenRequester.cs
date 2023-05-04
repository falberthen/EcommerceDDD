namespace EcommerceDDD.Core.Infrastructure.Identity;

public interface ITokenRequester
{
    Task<TokenResponse> GetApplicationTokenAsync(TokenIssuerSettings settings);
    Task<TokenResponse> GetUserTokenAsync(TokenIssuerSettings settings, string userName, string password);
    Task<string> GetUserTokenFromHttpContextAsync();
}