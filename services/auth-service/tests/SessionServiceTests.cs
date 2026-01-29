using System.Security.Claims;
using AuthService.Api.Domain;
using AuthService.Api.Repositories;
using AuthService.Api.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace AuthService.Api.Tests;

public class SessionServiceTests
{
    private readonly Mock<IUserProfileRepository> _repositoryMock = new();
    private readonly SessionService _sessionService;
    private readonly TokenValidationOptions _options = new()
    {
        Audience = "api://amcart-api"
    };

    public SessionServiceTests()
    {
        _sessionService = new SessionService(_repositoryMock.Object, Microsoft.Extensions.Options.Options.Create(_options));
    }

    [Fact]
    public async Task CreateOrUpdateSessionAsync_Persists_New_User()
    {
        // Arrange
        var principal = BuildPrincipal("user-123", "jane@example.com", "Jane Doe", new[] { "Catalog.Read" }, audience: "api://amcart-api");
        _repositoryMock.Setup(r => r.GetByAadObjectIdAsync("user-123", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        _repositoryMock.Setup(r => r.UpsertAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile profile, CancellationToken _) => profile);

        // Act
        var response = await _sessionService.CreateOrUpdateSessionAsync(principal, CancellationToken.None);

        // Assert
        response.AadObjectId.Should().Be("user-123");
        response.Email.Should().Be("jane@example.com");
        response.Roles.Should().Contain("Catalog.Read");
        _repositoryMock.Verify(r => r.UpsertAsync(It.Is<UserProfile>(p => p.AadObjectId == "user-123"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrUpdateSessionAsync_Updates_Existing_User()
    {
        // Arrange
        var existing = new UserProfile
        {
            Id = Guid.NewGuid(),
            AadObjectId = "user-123",
            Email = "old@example.com",
            DisplayName = "Old Name",
            Roles = new[] { "Catalog.Read" },
            CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
            LastLoginAtUtc = DateTime.UtcNow.AddDays(-1)
        };

        var principal = BuildPrincipal("user-123", "new@example.com", "New Name", new[] { "Order.Write" }, audience: "api://amcart-api");

        _repositoryMock.Setup(r => r.GetByAadObjectIdAsync("user-123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpsertAsync(existing, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var response = await _sessionService.CreateOrUpdateSessionAsync(principal, CancellationToken.None);

        // Assert
        response.Email.Should().Be("new@example.com");
        response.Roles.Should().Contain("Order.Write");
        _repositoryMock.Verify(r => r.UpsertAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProfileAsync_Returns_Null_When_Claim_Missing()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }, "Test"));

        var profile = await _sessionService.GetProfileAsync(principal, CancellationToken.None);

        profile.Should().BeNull();
        _repositoryMock.Verify(r => r.GetByAadObjectIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrUpdateSessionAsync_Throws_For_Invalid_Audience()
    {
        var principal = BuildPrincipal("user-abc", "user@example.com", "User", Array.Empty<string>(), audience: "wrong");

        var act = async () => await _sessionService.CreateOrUpdateSessionAsync(principal, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    private static ClaimsPrincipal BuildPrincipal(string oid, string email, string displayName, IEnumerable<string> roles, string? audience = null)
    {
        var claims = new List<Claim>
        {
            new("oid", oid),
            new(ClaimTypes.Email, email),
            new("name", displayName)
        };
        if (!string.IsNullOrWhiteSpace(audience))
        {
            claims.Add(new Claim("aud", audience));
        }
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}
