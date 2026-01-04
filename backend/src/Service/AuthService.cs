using Microsoft.EntityFrameworkCore;
using ProjectX.API.Data;
using ProjectX.API.Data.DTOs.Auth;
using ProjectX.API.Data.Models;

namespace ProjectX.API.Service;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponseDto?> AdminLoginAsync(LoginDto loginDto)
    {
        try
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == loginDto.Email && a.IsActive);

            if (admin == null)
            {
                _logger.LogWarning("Admin login failed: Email not found - {Email}", loginDto.Email);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, admin.PasswordHash))
            {
                _logger.LogWarning("Admin login failed: Invalid password - {Email}", loginDto.Email);
                return null;
            }

            admin.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(admin.Id, admin.Email, "Admin");

            return new AuthResponseDto
            {
                Id = admin.Id,
                Email = admin.Email,
                FullName = admin.Username,
                Token = token,
                Role = "Admin"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin login");
            throw;
        }
    }

    public async Task<AuthResponseDto?> UserLoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                _logger.LogWarning("User login failed: Email not found - {Email}", loginDto.Email);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("User login failed: Invalid password - {Email}", loginDto.Email);
                return null;
            }

            var token = _tokenService.GenerateToken(user.Id, user.Email, "User");

            return new AuthResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
                Role = "User"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            throw;
        }
    }

    public async Task<AuthResponseDto> UserRegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            // Create new user
            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FullName = registerDto.FullName,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                City = registerDto.City,
                Country = registerDto.Country,
                PostalCode = registerDto.PostalCode,
                CreateAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user.Id, user.Email, "User");

            return new AuthResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
                Role = "User"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            throw;
        }
    }
}
