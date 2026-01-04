namespace ProjectX.API.Service;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile file, string folder);
    Task<bool> DeleteImageAsync(string imagePath);
    bool ValidateImage(IFormFile file);
}
