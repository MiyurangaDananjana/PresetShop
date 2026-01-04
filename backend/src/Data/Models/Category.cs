using System.ComponentModel.DataAnnotations;

namespace ProjectX.API.Data.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? CreatedAt { get; set; }

    public ICollection<Preset> Presets { get; set; } = new List<Preset>();
}
