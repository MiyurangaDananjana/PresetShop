namespace ProjectX.API.Service;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImageService> _logger;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB

    public ImageService(IWebHostEnvironment environment, ILogger<ImageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public bool ValidateImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return false;
        }

        // Check file size
        if (file.Length > _maxFileSize)
        {
            _logger.LogWarning("File size exceeds limit: {Size} bytes", file.Length);
            return false;
        }

        // Check file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Invalid file extension: {Extension}", extension);
            return false;
        }

        // Check MIME type
        var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            _logger.LogWarning("Invalid MIME type: {MimeType}", file.ContentType);
            return false;
        }

        return true;
    }

    public async Task<string> SaveImageAsync(IFormFile file, string folder)
    {
        if (!ValidateImage(file))
        {
            throw new ArgumentException("Invalid image file");
        }

        try
        {
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.ContentRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsPath);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path
            return $"/uploads/{folder}/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving image file");
            throw new Exception("Failed to save image", ex);
        }
    }

    public async Task<bool> DeleteImageAsync(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            return false;
        }

        try
        {
            var fullPath = Path.Combine(_environment.ContentRootPath, imagePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                _logger.LogInformation("Deleted image: {Path}", imagePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image: {Path}", imagePath);
            return false;
        }
    }
}
