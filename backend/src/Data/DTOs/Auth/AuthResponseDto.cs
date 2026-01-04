namespace ProjectX.API.Data.DTOs.Auth;

public class AuthResponseDto
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required string Token { get; set; }
    public required string Role { get; set; }
}
