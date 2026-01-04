using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectX.API.Data.Enums;

namespace ProjectX.API.Data.Models;


public class Product
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? BeforeImageUrl { get; set; }
    public string? AfterImageUrl { get; set; }
    public string? Category { get; set; }
    public int PresetCount { get; set; }
    public ProductEnum IsActive { get; set; }
    public DateTime? CreateAt { get; set; }


    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}