namespace iCarus.Src.Services.Interfaces;

public interface ICloudinaryServices
{
    Task<string> UploadImageAsync(IFormFile image);
}
