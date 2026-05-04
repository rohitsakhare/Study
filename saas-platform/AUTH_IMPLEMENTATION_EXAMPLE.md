// Sample JWT Authentication implementation
// Add this to your service controllers:

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;
using Shared.DTOs;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserService _userService;

    public AuthController(IJwtTokenService jwtTokenService, IUserService userService)
    {
        _jwtTokenService = jwtTokenService;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Verify user credentials
            var user = await _userService.GetUserByUsernameAsync(request.Username);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // Verify password (implement password verification)
            // if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            //     return Unauthorized(new { message = "Invalid credentials" });

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Username);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            return Ok(new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("refresh")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.RefreshToken, "your-secret");
            if (principal == null)
                return Unauthorized(new { message = "Invalid token" });

            var userId = principal.FindFirst("sub")?.Value;
            var username = principal.FindFirst("name")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "Invalid token" });

            var newToken = _jwtTokenService.GenerateToken(userId, username);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            return Ok(new LoginResponse
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });
        }
        catch
        {
            return Unauthorized(new { message = "Invalid token" });
        }
    }
}
