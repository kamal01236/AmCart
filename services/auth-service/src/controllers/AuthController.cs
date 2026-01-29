using System.Security.Claims;
using AuthService.Api.Services;
using AuthService.Api.Transport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public AuthController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost("session")]
    [Authorize]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSession(CancellationToken cancellationToken)
    {
        var response = await _sessionService.CreateOrUpdateSessionAsync(User, cancellationToken);
        return Ok(response);
    }

    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var profile = await _sessionService.GetProfileAsync(User, cancellationToken);
        if (profile is null)
        {
            return NotFound();
        }

        return Ok(profile);
    }
}
