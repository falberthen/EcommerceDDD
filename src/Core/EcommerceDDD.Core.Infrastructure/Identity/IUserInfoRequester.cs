namespace EcommerceDDD.Core.Infrastructure.Identity;

public interface IUserInfoRequester
{
	Task<UserInfo?> RequestUserInfoAsync();
}