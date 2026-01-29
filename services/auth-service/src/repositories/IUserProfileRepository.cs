using AuthService.Api.Domain;

namespace AuthService.Api.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByAadObjectIdAsync(string aadObjectId, CancellationToken cancellationToken);
    Task<UserProfile> UpsertAsync(UserProfile profile, CancellationToken cancellationToken);
}
