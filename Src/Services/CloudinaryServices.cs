using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using iCarus.Src.Services.Interfaces;

namespace iCarus.Src.Services;

public class CloudinaryServices(IConfiguration config) : ICloudinaryServices
{
    private readonly Cloudinary _cloudinary = new(new Account(
        config["Cloudinary:CloudName"],
        config["Cloudinary:ApiKey"],
        config["Cloudinary:ApiSecret"]
    ));

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        await using var stream = image.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, stream),
            Folder = "iCarus"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception($"Error al subir imagen: {result.Error.Message}");

        return result.SecureUrl.ToString();
    }
}
