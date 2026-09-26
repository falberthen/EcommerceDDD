namespace EcommerceDDD.ServiceClients.Services.Identity;

public enum UserRegistrationStatus
{
    Registered,
    EmailTaken,
    Rejected
}

public record UserRegistrationResult(UserRegistrationStatus Status, string? Error = null);
