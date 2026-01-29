using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using AuthService.Api.Domain;
using AuthService.Api.Repositories;
using AuthService.Api.Services;

namespace AuthService.Api.Config;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Auth") ??
                throw new InvalidOperationException("Auth connection string missing");
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure();
            });
        });

        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ISessionService, SessionService>();

        services.AddOptions<TokenValidationOptions>()
            .Configure(options =>
            {
                var section = configuration.GetSection("AzureAd");
                options.ClientId = section["ClientId"] ?? string.Empty;
                options.Authority = section["Authority"] ?? string.Empty;
                options.TenantId = section["TenantId"] ?? string.Empty;
                options.Audience = section["Audience"] ?? string.Empty;
            });

        return services;
    }
}
