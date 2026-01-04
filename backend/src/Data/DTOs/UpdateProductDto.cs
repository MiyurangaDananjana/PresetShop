using System.ComponentModel.DataAnnotations;

namespace ProjectX.API.Data.DTOs;

public class UpdateProductDto
{
    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(0.01, 999999.99)]
    public decimal? Price { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [Range(1, 10000)]
    public int? PresetCount { get; set; }

    public IFormFile? MainImage { get; set; }
    public IFormFile? BeforeImage { get; set; }
    public IFormFile? AfterImage { get; set; }

    public bool? IsActive { get; set; }
}
