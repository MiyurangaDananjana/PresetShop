using Microsoft.EntityFrameworkCore;
using ProjectX.API.Data;
using ProjectX.API.Data.DTOs;
using ProjectX.API.Data.Models;

namespace ProjectX.API.Service;

public class PresetService : IPresetService
{
    private readonly AppDbContext _context;
    private readonly IImageService _imageService;
    private readonly ILogger<PresetService> _logger;

    public PresetService(
        AppDbContext context,
        IImageService imageService,
        ILogger<PresetService> logger)
    {
        _context = context;
        _imageService = imageService;
        _logger = logger;
    }

    public async Task<IEnumerable<PresetDto>> GetAllPresetsAsync(int? categoryId = null)
    {
        var query = _context.Presets
            .Include(p => p.Category)
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var presets = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return presets.Select(MapToDto);
    }

    public async Task<PresetDto?> GetPresetByIdAsync(int id)
    {
        var preset = await _context.Presets
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        return preset != null ? MapToDto(preset) : null;
    }

    public async Task<PresetDto> CreatePresetAsync(CreatePresetDto dto)
    {
        try
        {
            var preset = new Preset
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Handle before image upload
            if (dto.BeforeImage != null)
            {
                preset.BeforeImageUrl = await _imageService.SaveImageAsync(dto.BeforeImage, "presets/before");
            }

            // Handle after image upload
            if (dto.AfterImage != null)
            {
                preset.AfterImageUrl = await _imageService.SaveImageAsync(dto.AfterImage, "presets/after");
            }

            // Handle preset file upload
            if (dto.PresetFile != null)
            {
                preset.PresetFileUrl = await SavePresetFileAsync(dto.PresetFile);
            }

            _context.Presets.Add(preset);
            await _context.SaveChangesAsync();

            // Reload with category
            await _context.Entry(preset).Reference(p => p.Category).LoadAsync();

            return MapToDto(preset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating preset");
            throw;
        }
    }

    public async Task<PresetDto?> UpdatePresetAsync(int id, UpdatePresetDto dto)
    {
        try
        {
            var preset = await _context.Presets.FindAsync(id);
            if (preset == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Name))
                preset.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Description))
                preset.Description = dto.Description;

            if (dto.Price.HasValue)
                preset.Price = dto.Price.Value;

            if (dto.CategoryId.HasValue)
                preset.CategoryId = dto.CategoryId.Value;

            if (dto.IsActive.HasValue)
                preset.IsActive = dto.IsActive.Value;

            // Update before image if provided
            if (dto.BeforeImage != null)
            {
                if (!string.IsNullOrEmpty(preset.BeforeImageUrl))
                {
                    await _imageService.DeleteImageAsync(preset.BeforeImageUrl);
                }
                preset.BeforeImageUrl = await _imageService.SaveImageAsync(dto.BeforeImage, "presets/before");
            }

            // Update after image if provided
            if (dto.AfterImage != null)
            {
                if (!string.IsNullOrEmpty(preset.AfterImageUrl))
                {
                    await _imageService.DeleteImageAsync(preset.AfterImageUrl);
                }
                preset.AfterImageUrl = await _imageService.SaveImageAsync(dto.AfterImage, "presets/after");
            }

            // Update preset file if provided
            if (dto.PresetFile != null)
            {
                if (!string.IsNullOrEmpty(preset.PresetFileUrl))
                {
                    await DeletePresetFileAsync(preset.PresetFileUrl);
                }
                preset.PresetFileUrl = await SavePresetFileAsync(dto.PresetFile);
            }

            preset.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Reload with category
            await _context.Entry(preset).Reference(p => p.Category).LoadAsync();

            return MapToDto(preset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preset {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeletePresetAsync(int id)
    {
        try
        {
            var preset = await _context.Presets.FindAsync(id);
            if (preset == null)
            {
                return false;
            }

            // Delete associated files
            if (!string.IsNullOrEmpty(preset.BeforeImageUrl))
            {
                await _imageService.DeleteImageAsync(preset.BeforeImageUrl);
            }

            if (!string.IsNullOrEmpty(preset.AfterImageUrl))
            {
                await _imageService.DeleteImageAsync(preset.AfterImageUrl);
            }

            if (!string.IsNullOrEmpty(preset.PresetFileUrl))
            {
                await DeletePresetFileAsync(preset.PresetFileUrl);
            }

            _context.Presets.Remove(preset);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting preset {Id}", id);
            throw;
        }
    }

    private async Task<string> SavePresetFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Invalid preset file");
        }

        var allowedExtensions = new[] { ".zip", ".lrtemplate", ".xmp", ".dng" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Invalid file type. Allowed: ZIP, LRTEMPLATE, XMP, DNG");
        }

        if (file.Length > 50 * 1024 * 1024) // 50MB limit
        {
            throw new ArgumentException("File size exceeds 50MB limit");
        }

        try
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "presets", "files");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/presets/files/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving preset file");
            throw new Exception("Failed to save preset file", ex);
        }
    }

    private async Task<bool> DeletePresetFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                _logger.LogInformation("Deleted preset file: {Path}", filePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting preset file: {Path}", filePath);
            return false;
        }
    }

    private static PresetDto MapToDto(Preset preset)
    {
        return new PresetDto
        {
            Id = preset.Id,
            Name = preset.Name,
            Description = preset.Description,
            Price = preset.Price,
            CategoryId = preset.CategoryId,
            CategoryName = preset.Category?.Name,
            BeforeImageUrl = preset.BeforeImageUrl,
            AfterImageUrl = preset.AfterImageUrl,
            PresetFileUrl = preset.PresetFileUrl,
            IsActive = preset.IsActive,
            CreatedAt = preset.CreatedAt
        };
    }
}
