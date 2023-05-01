namespace EcommerceDDD.IdentityServer.Services;

public class IdentityManager : IIdentityManager
{
    private readonly ITokenRequester _tokenRequester;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenIssuerSettings _issuerSettings;
    public readonly RoleManager<IdentityRole> _roleManager;

    public IdentityManager(
        ITokenRequester tokenRequester, 
        UserManager<ApplicationUser> userManager,
        IOptions<TokenIssuerSettings> issuerSettings, 
        RoleManager<IdentityRole> roleManager)
    {
        _tokenRequester = tokenRequester;
        _userManager = userManager;
        _issuerSettings = issuerSettings.Value;
        _roleManager = roleManager;
    }

    public async Task<TokenResponse> AuthUserByCredentials(LoginRequest request)
    {
        var response = await _tokenRequester.GetUserTokenAsync(
            _issuerSettings,
            request.Email,
            request.Password);

        return response;
    }

    public async Task<IdentityResult> RegisterNewUser(RegisterUserRequest request)
    {
        await AddDefaultRoles();

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
        };

        // Creating user
        var result = await _userManager
            .CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ApplicationException($"Can't create user for {user.Email}");

        // Adding role
        result = await _userManager
            .AddToRoleAsync(user, IdentityConfiguration.CustomerRole);
        if (!result.Succeeded)
            throw new ApplicationException($"Can't add role for {user.Email}");

        // Adding claims
        result = await _userManager.AddClaimsAsync(user,
            new Claim[]
            {
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.CustomerRole),
            });
        if (!result.Succeeded)
            throw new ApplicationException($"Can't add claims for {user.Email}");

        return result;
    }

    private async Task AddDefaultRoles()
    {
        var clientRole = await _roleManager
            .FindByNameAsync(IdentityConfiguration.CustomerRole);

        if (clientRole is null)
        {
            var result = await _roleManager
                .CreateAsync(new IdentityRole(IdentityConfiguration.CustomerRole));

            if (!result.Succeeded)
                throw new ApplicationException($"Can't add role {IdentityConfiguration.CustomerRole}");
        }

        await Task.CompletedTask;
    }
}
