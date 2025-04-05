namespace EcommerceDDD.Core.Infrastructure.Identity;

public interface ITokenRequester
{
    Task<TokenResponse?> GetApplicationTokenAsync();
    Task<TokenResponse?> GetUserTokenFromCredentialsAsync(string userName, string password);
    Task<string?> GetUserTokenFromHttpContextAsync();
}