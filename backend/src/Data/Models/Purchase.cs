using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectX.API.Data.Models;

public class Purchase
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int PresetId { get; set; }
    public Preset Preset { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }

    [StringLength(100)]
    public string? TransactionId { get; set; }

    public DateTime PurchasedAt { get; set; }
    public bool IsCompleted { get; set; } = true;
}
