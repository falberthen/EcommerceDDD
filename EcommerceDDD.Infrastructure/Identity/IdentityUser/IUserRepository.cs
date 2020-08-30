using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceDDD.Infrastructure.Identity.IdentityUser
{
    public interface IUserRepository 
    {
        Task<User> GetUserByEmail(string email);
    }
}
