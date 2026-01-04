using ProjectX.API.Data.DTOs.Auth;

namespace ProjectX.API.Service;

public interface IAuthService
{
    Task<AuthResponseDto?> AdminLoginAsync(LoginDto loginDto);
    Task<AuthResponseDto?> UserLoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> UserRegisterAsync(RegisterDto registerDto);
}
