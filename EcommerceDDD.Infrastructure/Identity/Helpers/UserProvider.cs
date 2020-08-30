using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace EcommerceDDD.Infrastructure.Identity.Helpers
{
    public class UserProvider : IUserProvider
    {
        private readonly IHttpContextAccessor _context;
        
        public UserProvider(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Guid GetUserId()
        {
            var userIdString =_context.HttpContext.User.Claims
                       .First(i => i.Type == ClaimTypes.NameIdentifier).Value;
            Guid userId;

            Guid.TryParse(userIdString, out userId);
            return userId;
        }
    }
}
