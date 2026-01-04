using ProjectX.API.Data.DTOs;

namespace ProjectX.API.Service;

public interface IPurchaseService
{
    Task<PurchaseDto> CreatePurchaseAsync(int userId, CreatePurchaseDto dto);
    Task<IEnumerable<PurchaseDto>> GetUserPurchasesAsync(int userId);
    Task<bool> HasUserPurchasedPresetAsync(int userId, int presetId);
    Task<string?> GetDownloadUrlAsync(int userId, int presetId);
}
