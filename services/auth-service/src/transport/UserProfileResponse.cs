namespace AuthService.Api.Transport;

public record UserProfileResponse(
    Guid Id,
    string AadObjectId,
    string DisplayName,
    string Email,
    IReadOnlyCollection<string> Roles,
    DateTime CreatedAtUtc,
    DateTime LastLoginAtUtc);
