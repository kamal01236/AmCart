namespace AuthService.Api.Domain;

public class UserProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AadObjectId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastLoginAtUtc { get; set; } = DateTime.UtcNow;
}
