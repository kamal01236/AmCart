using System.Security.Claims;
using AuthService.Api.Transport;

namespace AuthService.Api.Services;

public interface ISessionService
{
    Task<SessionResponse> CreateOrUpdateSessionAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
    Task<UserProfileResponse?> GetProfileAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
}
