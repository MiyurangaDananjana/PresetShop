namespace ProjectX.API.Data.DTOs;

public class PurchaseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PresetId { get; set; }
    public string? PresetName { get; set; }
    public decimal PurchasePrice { get; set; }
    public string? TransactionId { get; set; }
    public DateTime PurchasedAt { get; set; }
    public bool IsCompleted { get; set; }
}

public class CreatePurchaseDto
{
    public int PresetId { get; set; }
}
