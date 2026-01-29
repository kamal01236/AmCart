using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Domain;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var profile = modelBuilder.Entity<UserProfile>();
        profile.ToTable("user_profiles");
        profile.HasKey(x => x.Id);
        profile.HasIndex(x => x.AadObjectId).IsUnique();
        profile.Property(x => x.AadObjectId).IsRequired().HasMaxLength(64);
        profile.Property(x => x.Email).IsRequired().HasMaxLength(256);
        profile.Property(x => x.DisplayName).IsRequired().HasMaxLength(256);
        profile.Property(x => x.CreatedAtUtc).IsRequired();
        profile.Property(x => x.LastLoginAtUtc).IsRequired();
        profile.Property(x => x.Roles)
            .HasConversion(
                roles => string.Join(',', roles ?? Array.Empty<string>()),
                value => string.IsNullOrWhiteSpace(value)
                    ? Array.Empty<string>()
                    : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .HasColumnType("text");
    }
}
