using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EcommerceDDD.Infrastructure.Identity.Users;

public interface IApplicationUser
{
    string Name { get; }
    string Email { get; }
    ClaimsPrincipal GetUser();
    bool IsAuthenticated();
    IEnumerable<Claim> GetClaimsIdentity();
}

public class ApplicationUser : IdentityUser<Guid>, IApplicationUser
{
    public string Name => GetName();

    public ApplicationUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public ClaimsPrincipal GetUser()
    {
        return _accessor?.HttpContext?.User as ClaimsPrincipal;
    }

    private string GetName()
    {
        return _accessor.HttpContext.User.Identity.Name ??
                _accessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    }

    public bool IsAuthenticated()
    {
        return _accessor.HttpContext.User.Identity.IsAuthenticated;
    }

    public IEnumerable<Claim> GetClaimsIdentity()
    {
        return _accessor.HttpContext.User.Claims;
    }


    private readonly IHttpContextAccessor _accessor;

    private ApplicationUser()
    {
    }
}