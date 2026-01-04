using System.ComponentModel.DataAnnotations;

namespace ProjectX.API.Data.DTOs.Auth;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }
}
