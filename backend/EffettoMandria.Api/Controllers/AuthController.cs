using EffettoMandria.Api.Models;
using EffettoMandria.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EffettoMandria.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService auth) : ControllerBase
{
    private readonly AuthService _auth = auth;

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest req)
    {
        try
        {
            var (user, token) = _auth.Register(req);
            return Ok(ToAuthResponse(user, token));
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        try
        {
            var (user, token) = _auth.Login(req);
            return Ok(ToAuthResponse(user, token));
        }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { error = ex.Message }); }
    }

    [HttpPost("guest")]
    public IActionResult Guest([FromBody] GuestRequest req)
    {
        try
        {
            var (user, token) = _auth.CreateGuest(req);
            return Ok(ToAuthResponse(user, token));
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        var user = HttpContext.GetCurrentUser(_auth);
        if (user is null) return Unauthorized();
        return Ok(new
        {
            user.Id,
            user.DisplayName,
            user.Username,
            user.IsGuest,
            currentGameId = user.CurrentGameId
        });
    }

    private static object ToAuthResponse(User user, Guid token) => new
    {
        token,
        user = new
        {
            user.Id,
            user.DisplayName,
            user.Username,
            user.IsGuest,
            currentGameId = user.CurrentGameId
        }
    };
}
