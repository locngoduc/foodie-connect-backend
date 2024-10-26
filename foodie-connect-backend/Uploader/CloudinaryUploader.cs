using System.Drawing.Imaging;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FluentEmail.Core;
using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Uploader;

public class CloudinaryUploader: IUploaderService
{
    private readonly Cloudinary _cloudinary = new();
    
    public List<string> AllowedExtensions { get; init; } = [".png", ".jpg", ".jpeg", ".webp"];
    public int MaxFileSize { get; init; } = 5 * 1024 * 1024; // 5MB

    private void _validateFile(IFormFile file)
    {
        var fileExtension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(fileExtension)) 
            throw new ArgumentOutOfRangeException($"Allowed file extensions: {string.Join(",", AllowedExtensions)}");
        
        var fileSize = file.Length;
        if (fileSize > MaxFileSize)
            throw new ArgumentOutOfRangeException($"File too big: {fileSize / (1024 ^ 2)}MB (Max: {MaxFileSize / (1024 ^ 2)}MB)");
    }

    public async Task<Result<string>> UploadImageAsync(IFormFile file, ImageFileOptions? options = null)
    {
        try
        {
            _validateFile(file);
        }
        catch (Exception e)
        {
            return Result<string>.Failure(AppError.ValidationError(e.Message));
        }

        var uploadParams = new ImageUploadParams()
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
        return Result<string>.Success(result.PublicId);
    }

    public async Task<Result<IList<string>>> UploadImagesAsync(IEnumerable<IFormFile> files, ImageFileOptions? options = null)
    {
        var fileList = files.ToList();
        var formFiles = fileList.ToList();
        if (formFiles.Count == 0) 
            return Result<IList<string>>.Failure(new AppError("NoImage", "No images provided"));

        foreach (var file in formFiles) try
            {
                _validateFile(file);
            }
            catch (Exception e)
            {
                return Result<IList<string>>.Failure(AppError.ValidationError(e.Message));
            }
        
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
        if (result.Error != null) return Result<bool>.Failure(AppError.ValidationError(result.Error.Message));
        return Result<bool>.Success(true);
    }
}