using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FluentEmail.Core;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;

namespace foodie_connect_backend.Modules.Uploader;

public class CloudinaryUploader : IUploaderService
{
    private readonly Cloudinary _cloudinary = new();

    public List<string> AllowedExtensions { get; init; } = [".png", ".jpg", ".jpeg", ".webp"];
    public int MaxFileSize { get; init; } = 5 * 1024 * 1024; // 5MB

    public Result<bool> ValidateFile(IFormFile file)
    {
        var fileExtension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(fileExtension))
            return Result<bool>.Failure(UploadError.TypeNotAllowed(fileExtension, AllowedExtensions));

        var fileSize = file.Length;
        if (fileSize > MaxFileSize)
            return Result<bool>.Failure(UploadError.ExceedMaxSize(fileSize, MaxFileSize));

        return Result<bool>.Success(true);
    }

    public async Task<Result<string>> UploadImageAsync(IFormFile file, ImageFileOptions? options = null)
    {
        var validationResult = ValidateFile(file);
        if (!validationResult.IsSuccess) return Result<string>.Failure(validationResult.Error);

        var uploadParams = new ImageUploadParams
        {
            // Default configuration
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            Format = "webp"
        };

        if (options is not null)
        {
            uploadParams.Format = options.Format;
            uploadParams.Folder = options.Folder;
            uploadParams.PublicId = options.PublicId;
        }

        var result = await _cloudinary.UploadAsync(uploadParams);
        return Result<string>.Success($"{result.PublicId}.{result.Version}");
    }

    public async Task<Result<IList<string>>> UploadImagesAsync(IEnumerable<IFormFile> files,
        ImageFileOptions? options = null)
    {
        var fileList = files.ToList();

        var uploadTasks = fileList.Select(file => UploadImageAsync(file, options));
        var results = await Task.WhenAll(uploadTasks);

        var publicIds = new List<string>();
        results.ForEach(result => publicIds.Add(result.Value));
        return Result<IList<string>>.Success(publicIds);
    }

    public async Task<Result<bool>> DeleteFileAsync(string fileId)
    {
        var deletionParams = new DeletionParams(fileId);

        var result = await _cloudinary.DestroyAsync(deletionParams);
        if (result.Error != null) return Result<bool>.Failure(AppError.InternalError());
        return Result<bool>.Success(true);
    }
}