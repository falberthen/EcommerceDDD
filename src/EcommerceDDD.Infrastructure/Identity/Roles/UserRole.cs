using Microsoft.AspNetCore.Identity;

namespace EcommerceDDD.Infrastructure.Identity.Roles;

public class UserRole : IdentityRole<Guid>
{
    public UserRole()
    {
        this.Id = Guid.NewGuid();
    }

    public UserRole(string name)
        : this()
    {
        this.Name = name;
    }
}