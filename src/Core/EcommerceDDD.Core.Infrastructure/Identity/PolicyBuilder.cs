namespace EcommerceDDD.Core.Infrastructure.Identity;

public static class PolicyBuilder
{
    public const string ReadPolicy = "ReadAccess";
    public const string WritePolicy = "WriteAccess";
    public const string DeletePolicy = "WriteAccess";

    public static AuthorizationPolicy ReadAccess =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim("scope", "read")
            .Build();

    public static AuthorizationPolicy WriteAccess =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim("scope", "write")
            .Build();

    public static AuthorizationPolicy DeleteAccess =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim("scope", "delete")
            .Build();
}
