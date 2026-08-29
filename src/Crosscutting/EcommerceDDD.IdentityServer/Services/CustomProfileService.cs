namespace EcommerceDDD.IdentityServer.Services;

public class CustomProfileService(
	UserManager<ApplicationUser> userMgr,
	RoleManager<IdentityRole> roleMgr,
	IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory) : IProfileService
{
	private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory
		?? throw new ArgumentNullException(nameof(userClaimsPrincipalFactory));
	private readonly UserManager<ApplicationUser> _userMgr = userMgr
		?? throw new ArgumentNullException(nameof(userMgr));
	private readonly RoleManager<IdentityRole> _roleMgr = roleMgr
		?? throw new ArgumentNullException(nameof(roleMgr));

	public async Task GetProfileDataAsync(ProfileDataRequestContext context,
		CancellationToken cancellationToken = default)
	{		
		cancellationToken.ThrowIfCancellationRequested();

		string sub = context.Subject.GetSubjectId();
		ApplicationUser? user = await _userMgr.FindByIdAsync(sub);

		if (user is null)
			return;

		ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);

		List<Claim> claims = userClaims.Claims.ToList();
		claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

		if (!string.IsNullOrEmpty(user.Email))
			claims.Add(new Claim(JwtClaimTypes.Email, user.Email));

		if (_userMgr.SupportsUserRole)
		{
			IList<string> roles = await _userMgr.GetRolesAsync(user);
			foreach (var rolename in roles)
			{
				claims.Add(new Claim(JwtClaimTypes.Role, rolename));
				if (_roleMgr.SupportsRoleClaims)
				{
					IdentityRole? role = await _roleMgr.FindByNameAsync(rolename);
					if (role != null)
					{
						claims.AddRange(await _roleMgr.GetClaimsAsync(role));
					}
				}
			}
		}

		context.IssuedClaims = claims;
	}

	public async Task IsActiveAsync(IsActiveContext context,
		CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		string sub = context.Subject.GetSubjectId();
		ApplicationUser? user = await _userMgr.FindByIdAsync(sub);
		context.IsActive = user is not null;
	}
}