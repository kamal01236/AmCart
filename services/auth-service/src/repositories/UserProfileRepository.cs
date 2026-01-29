using AuthService.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly AuthDbContext _dbContext;

    public UserProfileRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfile?> GetByAadObjectIdAsync(string aadObjectId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.AadObjectId == aadObjectId, cancellationToken);
    }

    public async Task<UserProfile> UpsertAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.UserProfiles
            .FirstOrDefaultAsync(u => u.AadObjectId == profile.AadObjectId, cancellationToken);

        if (existing is null)
        {
            await _dbContext.UserProfiles.AddAsync(profile, cancellationToken);
        }
        else
        {
            existing.DisplayName = profile.DisplayName;
            existing.Email = profile.Email;
            existing.Roles = profile.Roles;
            existing.LastLoginAtUtc = profile.LastLoginAtUtc;
            profile = existing;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return profile;
    }
}
