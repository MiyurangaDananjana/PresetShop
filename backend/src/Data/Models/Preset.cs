using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectX.API.Data.Enums;

namespace ProjectX.API.Data.Models;

public class Preset
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public required string Name { get; set; }

    [Required]
    [StringLength(2000)]
    public required string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public string? BeforeImageUrl { get; set; }
    public string? AfterImageUrl { get; set; }
    public string? PresetFileUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
