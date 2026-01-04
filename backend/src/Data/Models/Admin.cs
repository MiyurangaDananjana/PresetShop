using System.ComponentModel.DataAnnotations;

namespace ProjectX.API.Data.Models;

public class Admin
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}
