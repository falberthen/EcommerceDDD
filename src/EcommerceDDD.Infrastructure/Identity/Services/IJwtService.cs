using System.Threading.Tasks;

namespace EcommerceDDD.Infrastructure.Identity.Services;

public interface IJwtService
{
    Task<string> GenerateJwt(string email);
}