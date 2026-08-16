namespace EcommerceDDD.Core.Infrastructure.Identity;

public interface IUserInfoRequester
{
	UserInfo GetCurrentUser();
}
