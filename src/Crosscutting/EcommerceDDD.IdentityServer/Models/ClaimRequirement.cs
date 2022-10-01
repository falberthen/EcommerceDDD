using Microsoft.AspNetCore.Authorization;

namespace EcommerceDDD.IdentityServer.Models;

public record class ClaimRequirement : IAuthorizationRequirement
{
    public ClaimRequirement(string claimName, string claimValue)
    {
        ClaimName = claimName;
        ClaimValue = claimValue;
    }

    public string ClaimName { get; set; }
    public string ClaimValue { get; set; }
}

public class ClaimsRequirementHandler : AuthorizationHandler<ClaimRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ClaimRequirement requirement)
    {
        var claim = context.User.Claims
            .FirstOrDefault(c => c.Type == requirement.ClaimName);

        if (claim is not null && claim.Value.Contains(requirement.ClaimValue))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}