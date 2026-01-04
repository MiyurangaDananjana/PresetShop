using ProjectX.API.Data.Models;

namespace ProjectX.API.Data;

public static class DbInitializer
{
    public static async Task SeedDefaultAdminAsync(AppDbContext context)
    {
        // Check if admin already exists
        if (context.Admins.Any())
        {
            return; // Admin already seeded
        }

        var defaultAdmin = new Admin
        {
            Username = "admin",
            Email = "admin@presetshop.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        context.Admins.Add(defaultAdmin);
        await context.SaveChangesAsync();
    }

    public static async Task SeedDefaultCategoriesAsync(AppDbContext context)
    {
        // Check if categories already exist
        if (context.Categories.Any())
        {
            return;
        }

        var categories = new[]
        {
            new Category
            {
                Name = "Mobile Presets",
                Description = "Lightroom presets optimized for mobile photography",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Lightroom Presets",
                Description = "Professional Lightroom presets for desktop",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
    }
}
