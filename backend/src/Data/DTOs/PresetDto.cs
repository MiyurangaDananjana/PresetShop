using System.ComponentModel.DataAnnotations;

namespace ProjectX.API.Data.DTOs;

public class PresetDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? BeforeImageUrl { get; set; }
    public string? AfterImageUrl { get; set; }
    public string? PresetFileUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class CreatePresetDto
{
    [Required]
    [StringLength(200)]
    public required string Name { get; set; }

    [Required]
    [StringLength(2000)]
    public required string Description { get; set; }

    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }

    public int? CategoryId { get; set; }

    public IFormFile? BeforeImage { get; set; }
    public IFormFile? AfterImage { get; set; }
    public IFormFile? PresetFile { get; set; }
}

public class UpdatePresetDto
{
    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(0.01, 999999.99)]
    public decimal? Price { get; set; }

    public int? CategoryId { get; set; }
    public bool? IsActive { get; set; }

    public IFormFile? BeforeImage { get; set; }
    public IFormFile? AfterImage { get; set; }
    public IFormFile? PresetFile { get; set; }
}
