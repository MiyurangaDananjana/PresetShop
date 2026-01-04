using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.API.Data.DTOs;
using ProjectX.API.Service;

namespace ProjectX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;
    private readonly ILogger<PurchasesController> _logger;

    public PurchasesController(IPurchaseService purchaseService, ILogger<PurchasesController> logger)
    {
        _purchaseService = purchaseService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new purchase (demo payment)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PurchaseDto>> CreatePurchase([FromBody] CreatePurchaseDto dto)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var purchase = await _purchaseService.CreatePurchaseAsync(userId.Value, dto);
            return Ok(purchase);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase");
            return StatusCode(500, new { message = "An error occurred while processing your purchase" });
        }
    }

    /// <summary>
    /// Get all purchases for the logged-in user
    /// </summary>
    [HttpGet("my-purchases")]
    public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetMyPurchases()
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var purchases = await _purchaseService.GetUserPurchasesAsync(userId.Value);
            return Ok(purchases);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user purchases");
            return StatusCode(500, new { message = "An error occurred while fetching your purchases" });
        }
    }

    /// <summary>
    /// Download a purchased preset
    /// </summary>
    [HttpGet("download/{presetId}")]
    public async Task<ActionResult> DownloadPreset(int presetId)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var hasPurchased = await _purchaseService.HasUserPurchasedPresetAsync(userId.Value, presetId);
            if (!hasPurchased)
            {
                return Forbidden(new { message = "You have not purchased this preset" });
            }

            var downloadUrl = await _purchaseService.GetDownloadUrlAsync(userId.Value, presetId);
            if (string.IsNullOrEmpty(downloadUrl))
            {
                return NotFound(new { message = "Preset file not found" });
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), downloadUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "Preset file not found" });
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading preset {PresetId}", presetId);
            return StatusCode(500, new { message = "An error occurred while downloading the preset" });
        }
    }

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private ObjectResult Forbidden(object value)
    {
        return StatusCode(403, value);
    }
}
