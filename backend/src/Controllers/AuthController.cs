using Microsoft.AspNetCore.Mvc;
using ProjectX.API.Data.DTOs.Auth;
using ProjectX.API.Service;

namespace ProjectX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Admin login endpoint
    /// </summary>
    [HttpPost("admin/login")]
    public async Task<ActionResult<AuthResponseDto>> AdminLogin([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.AdminLoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin login");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// User login endpoint
    /// </summary>
    [HttpPost("user/login")]
    public async Task<ActionResult<AuthResponseDto>> UserLogin([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.UserLoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// User registration endpoint
    /// </summary>
    [HttpPost("user/register")]
    public async Task<ActionResult<AuthResponseDto>> UserRegister([FromBody] RegisterDto registerDto)
    {
        try
        {
            var result = await _authService.UserRegisterAsync(registerDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }
}
