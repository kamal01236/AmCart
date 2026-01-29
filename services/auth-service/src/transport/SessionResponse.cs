namespace AuthService.Api.Transport;

public record SessionResponse(
    string AadObjectId,
    string DisplayName,
    string Email,
    IReadOnlyCollection<string> Roles,
    DateTime LastLoginAtUtc);
