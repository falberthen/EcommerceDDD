using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Infrastructure.Identity.Helpers
{
    public interface IUserProvider
    {
        Guid GetUserId();
    }
}
