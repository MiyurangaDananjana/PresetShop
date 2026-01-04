namespace ProjectX.API.Data.DTOs;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? BeforeImageUrl { get; set; }
    public string? AfterImageUrl { get; set; }
    public string? Category { get; set; }
    public int PresetCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
}
