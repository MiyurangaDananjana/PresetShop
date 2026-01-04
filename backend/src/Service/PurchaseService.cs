using Microsoft.EntityFrameworkCore;
using ProjectX.API.Data;
using ProjectX.API.Data.DTOs;
using ProjectX.API.Data.Models;

namespace ProjectX.API.Service;

public class PurchaseService : IPurchaseService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PurchaseService> _logger;

    public PurchaseService(AppDbContext context, ILogger<PurchaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PurchaseDto> CreatePurchaseAsync(int userId, CreatePurchaseDto dto)
    {
        try
        {
            var preset = await _context.Presets.FindAsync(dto.PresetId);
            if (preset == null)
            {
                throw new InvalidOperationException("Preset not found");
            }

            if (!preset.IsActive)
            {
                throw new InvalidOperationException("Preset is not available for purchase");
            }

            // Check if user already purchased this preset
            var existingPurchase = await _context.Purchases
                .FirstOrDefaultAsync(p => p.UserId == userId && p.PresetId == dto.PresetId);

            if (existingPurchase != null)
            {
                throw new InvalidOperationException("You have already purchased this preset");
            }

            // Create purchase record
            var purchase = new Purchase
            {
                UserId = userId,
                PresetId = dto.PresetId,
                PurchasePrice = preset.Price,
                TransactionId = $"DEMO-{Guid.NewGuid().ToString("N")[..12].ToUpper()}",
                PurchasedAt = DateTime.UtcNow,
                IsCompleted = true
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Reload with preset details
            await _context.Entry(purchase)
                .Reference(p => p.Preset)
                .LoadAsync();

            return MapToDto(purchase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase for user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseDto>> GetUserPurchasesAsync(int userId)
    {
        var purchases = await _context.Purchases
            .Include(p => p.Preset)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PurchasedAt)
            .ToListAsync();

        return purchases.Select(MapToDto);
    }

    public async Task<bool> HasUserPurchasedPresetAsync(int userId, int presetId)
    {
        return await _context.Purchases
            .AnyAsync(p => p.UserId == userId && p.PresetId == presetId && p.IsCompleted);
    }

    public async Task<string?> GetDownloadUrlAsync(int userId, int presetId)
    {
        var purchase = await _context.Purchases
            .Include(p => p.Preset)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.PresetId == presetId && p.IsCompleted);

        if (purchase == null || purchase.Preset == null)
        {
            return null;
        }

        return purchase.Preset.PresetFileUrl;
    }

    private static PurchaseDto MapToDto(Purchase purchase)
    {
        return new PurchaseDto
        {
            Id = purchase.Id,
            UserId = purchase.UserId,
            PresetId = purchase.PresetId,
            PresetName = purchase.Preset?.Name,
            PurchasePrice = purchase.PurchasePrice,
            TransactionId = purchase.TransactionId,
            PurchasedAt = purchase.PurchasedAt,
            IsCompleted = purchase.IsCompleted
        };
    }
}
