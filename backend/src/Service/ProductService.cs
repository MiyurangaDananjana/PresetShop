using Microsoft.EntityFrameworkCore;
using ProjectX.API.Data;
using ProjectX.API.Data.DTOs;
using ProjectX.API.Data.Enums;
using ProjectX.API.Data.Models;

namespace ProjectX.API.Service;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly IImageService _imageService;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        AppDbContext context,
        IImageService imageService,
        ILogger<ProductService> logger)
    {
        _context = context;
        _imageService = imageService;
        _logger = logger;
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
    {
        try
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                PresetCount = dto.PresetCount,
                IsActive = ProductEnum.IsActive,
                CreateAt = DateTime.UtcNow
            };

            // Handle main image upload
            if (dto.MainImage != null)
            {
                product.ImageUrl = await _imageService.SaveImageAsync(dto.MainImage, "products");
            }

            // Handle before image upload
            if (dto.BeforeImage != null)
            {
                product.BeforeImageUrl = await _imageService.SaveImageAsync(dto.BeforeImage, "before");
            }

            // Handle after image upload
            if (dto.AfterImage != null)
            {
                product.AfterImageUrl = await _imageService.SaveImageAsync(dto.AfterImage, "after");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return MapToDto(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            throw;
        }
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        var products = await _context.Products
            .Where(p => p.IsActive == ProductEnum.IsActive)
            .OrderByDescending(p => p.CreateAt)
            .ToListAsync();

        return products.Select(MapToDto);
    }

    public async Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return null;
            }

            // Update basic fields
            if (!string.IsNullOrEmpty(dto.Name))
                product.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Description))
                product.Description = dto.Description;

            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;

            if (!string.IsNullOrEmpty(dto.Category))
                product.Category = dto.Category;

            if (dto.PresetCount.HasValue)
                product.PresetCount = dto.PresetCount.Value;

            if (dto.IsActive.HasValue)
                product.IsActive = dto.IsActive.Value ? ProductEnum.IsActive : ProductEnum.IsDeActive;

            // Update main image if provided
            if (dto.MainImage != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(product.ImageUrl);
                }
                product.ImageUrl = await _imageService.SaveImageAsync(dto.MainImage, "products");
            }

            // Update before image if provided
            if (dto.BeforeImage != null)
            {
                if (!string.IsNullOrEmpty(product.BeforeImageUrl))
                {
                    await _imageService.DeleteImageAsync(product.BeforeImageUrl);
                }
                product.BeforeImageUrl = await _imageService.SaveImageAsync(dto.BeforeImage, "before");
            }

            // Update after image if provided
            if (dto.AfterImage != null)
            {
                if (!string.IsNullOrEmpty(product.AfterImageUrl))
                {
                    await _imageService.DeleteImageAsync(product.AfterImageUrl);
                }
                product.AfterImageUrl = await _imageService.SaveImageAsync(dto.AfterImage, "after");
            }

            await _context.SaveChangesAsync();
            return MapToDto(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            // Delete associated images
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                await _imageService.DeleteImageAsync(product.ImageUrl);
            }

            if (!string.IsNullOrEmpty(product.BeforeImageUrl))
            {
                await _imageService.DeleteImageAsync(product.BeforeImageUrl);
            }

            if (!string.IsNullOrEmpty(product.AfterImageUrl))
            {
                await _imageService.DeleteImageAsync(product.AfterImageUrl);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {Id}", id);
            throw;
        }
    }

    private static ProductResponseDto MapToDto(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            BeforeImageUrl = product.BeforeImageUrl,
            AfterImageUrl = product.AfterImageUrl,
            Category = product.Category,
            PresetCount = product.PresetCount,
            IsActive = product.IsActive == ProductEnum.IsActive,
            CreatedAt = product.CreateAt
        };
    }
}
