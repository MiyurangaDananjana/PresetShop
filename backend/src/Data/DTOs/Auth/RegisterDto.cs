using System.ComponentModel.DataAnnotations;

namespace ProjectX.API.Data.DTOs.Auth;

public class RegisterDto
{
    [Required]
    [StringLength(100)]
    public required string FullName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }

    [Required]
    [StringLength(20)]
    public required string PhoneNumber { get; set; }

    [Required]
    [StringLength(200)]
    public required string Address { get; set; }

    [Required]
    [StringLength(100)]
    public required string City { get; set; }

    [Required]
    [StringLength(100)]
    public required string Country { get; set; }

    [Required]
    [StringLength(20)]
    public required string PostalCode { get; set; }
}
