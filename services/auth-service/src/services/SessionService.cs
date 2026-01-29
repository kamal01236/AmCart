using System.Security.Claims;
using AuthService.Api.Domain;
using AuthService.Api.Repositories;
using AuthService.Api.Transport;

namespace AuthService.Api.Services;

public class SessionService : ISessionService
{
    private readonly IUserProfileRepository _repository;
    private readonly TokenValidationOptions _options;

    public SessionService(IUserProfileRepository repository, Microsoft.Extensions.Options.IOptions<TokenValidationOptions> options)
    {
        _repository = repository;
        _options = options.Value;
    }

    public async Task<SessionResponse> CreateOrUpdateSessionAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var aadObjectId = principal.FindFirst("oid")?.Value ??
                          principal.FindFirst("sub")?.Value ??
                          throw new InvalidOperationException("Token missing object identifier (oid)");

        var email = principal.FindFirst(ClaimTypes.Email)?.Value ??
                    principal.FindFirst("preferred_username")?.Value ??
                    principal.Identity?.Name ??
                    "unknown@local";

        var displayName = principal.FindFirst("name")?.Value ?? email;

        var roleClaims = principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value)
            .Concat(principal.FindAll("roles").Select(c => c.Value))
            .Concat(principal.FindAll("scp").SelectMany(scope => scope.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        EnsureAudience(principal);

        var profile = await _repository.GetByAadObjectIdAsync(aadObjectId, cancellationToken);
        if (profile is null)
        {
            profile = new UserProfile
            {
                AadObjectId = aadObjectId,
                Email = email,
                DisplayName = displayName,
                Roles = roleClaims,
                CreatedAtUtc = DateTime.UtcNow,
                LastLoginAtUtc = DateTime.UtcNow
            };
        }
        else
        {
            profile.DisplayName = displayName;
            profile.Email = email;
            profile.Roles = roleClaims;
            profile.LastLoginAtUtc = DateTime.UtcNow;
        }

        var saved = await _repository.UpsertAsync(profile, cancellationToken);

        return new SessionResponse(
            saved.AadObjectId,
            saved.DisplayName,
            saved.Email,
            saved.Roles,
            saved.LastLoginAtUtc);
    }

    public async Task<UserProfileResponse?> GetProfileAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var aadObjectId = principal.FindFirst("oid")?.Value ?? principal.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(aadObjectId))
        {
            return null;
        }

        var profile = await _repository.GetByAadObjectIdAsync(aadObjectId, cancellationToken);
        return profile is null
            ? null
            : new UserProfileResponse(
                profile.Id,
                profile.AadObjectId,
                profile.DisplayName,
                profile.Email,
                profile.Roles,
                profile.CreatedAtUtc,
                profile.LastLoginAtUtc);
    }

    private void EnsureAudience(ClaimsPrincipal principal)
    {
        if (string.IsNullOrWhiteSpace(_options.Audience))
        {
            return;
        }

        var audienceClaim = principal.FindFirst("aud")?.Value;
        if (!string.Equals(audienceClaim, _options.Audience, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Token audience mismatch");
        }
    }
}
