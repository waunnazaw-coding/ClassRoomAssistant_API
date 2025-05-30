using ClassRoomClone_App.Server.Helpers;

namespace ClassRoomClone_App.Server.Services.Implements;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _assetFolder;

    public CloudinaryService(Cloudinary cloudinary, IOptions<CloudinarySettings> config)
    {
        _cloudinary = cloudinary;
        _assetFolder = config.Value.Folder;  // Logical folder for asset organization
    }

    // Upload Image (jpg, png, etc.)
    public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            AssetFolder = _assetFolder,
            PublicIdPrefix = _assetFolder,
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = false
        };

        return await _cloudinary.UploadAsync(uploadParams);
    }

    // Upload Video (mp4, mov, etc.)
    public async Task<VideoUploadResult> UploadVideoAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var uploadParams = new VideoUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            AssetFolder = _assetFolder,
            PublicIdPrefix = _assetFolder,
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = false
        };

        return await _cloudinary.UploadAsync(uploadParams);
    }

    // Upload Raw File (pdf, txt, docx, etc.)
    public async Task<RawUploadResult> UploadRawFileAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var uploadParams = new RawUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            AssetFolder = _assetFolder,
            PublicIdPrefix = _assetFolder,
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = false
        };

        return await _cloudinary.UploadAsync(uploadParams);
    }
}
