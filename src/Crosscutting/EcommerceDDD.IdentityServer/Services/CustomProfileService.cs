using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using EcommerceDDD.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EcommerceDDD.IdentityServer.Services;

public class CustomProfileService : IProfileService
{

    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly UserManager<ApplicationUser> _userMgr;
    private readonly RoleManager<IdentityRole> _roleMgr;

    public CustomProfileService(
        UserManager<ApplicationUser> userMgr,
        RoleManager<IdentityRole> roleMgr,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
    {
        _userMgr = userMgr;
        _roleMgr = roleMgr;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        string sub = context.Subject.GetSubjectId();
        ApplicationUser user = await _userMgr.FindByIdAsync(sub);
        ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);

        List<Claim> claims = userClaims.Claims.ToList();
        claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
        claims.Add(new Claim(JwtClaimTypes.Email, user.Email));

        if (_userMgr.SupportsUserRole)
        {
            IList<string> roles = await _userMgr.GetRolesAsync(user);
            foreach (var rolename in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, rolename));
                if (_roleMgr.SupportsRoleClaims)
                {
                    IdentityRole role = await _roleMgr.FindByNameAsync(rolename);
                    if (role != null)
                    {
                        claims.AddRange(await _roleMgr.GetClaimsAsync(role));
                    }
                }
            }
        }

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        string sub = context.Subject.GetSubjectId();
        ApplicationUser user = await _userMgr.FindByIdAsync(sub);
        context.IsActive = user != null;
    }
}

