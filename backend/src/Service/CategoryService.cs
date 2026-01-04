using Microsoft.EntityFrameworkCore;
using ProjectX.API.Data;
using ProjectX.API.Data.DTOs;
using ProjectX.API.Data.Models;

namespace ProjectX.API.Service;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(AppDbContext context, ILogger<CategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        try
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == dto.Name);

            if (existingCategory != null)
            {
                throw new InvalidOperationException("Category with this name already exists");
            }

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            throw;
        }
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Name))
            {
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name == dto.Name && c.Id != id);

                if (existingCategory != null)
                {
                    throw new InvalidOperationException("Category with this name already exists");
                }

                category.Name = dto.Name;
            }

            if (dto.Description != null)
            {
                category.Description = dto.Description;
            }

            if (dto.IsActive.HasValue)
            {
                category.IsActive = dto.IsActive.Value;
            }

            await _context.SaveChangesAsync();
            return MapToDto(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {Id}", id);
            throw;
        }
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }
}
