using System;
using System.Linq;
using System.Threading.Tasks;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Identity.IdentityUser
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityContext _dbContext;

        public UserRepository(IdentityContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}
