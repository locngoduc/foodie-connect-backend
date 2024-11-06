using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Modules.Uploader;

public interface IUploaderService
{
    List<string> AllowedExtensions { get; init; }
    int MaxFileSize { get; init; }
    
    public void ValidateFile(IFormFile file);
    
    /// <summary>
    /// Uploads a single file to the storage service
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="options">File upload configuration options</param>
    /// <returns>The identifier of the uploaded file</returns>
    Task<Result<string>> UploadImageAsync(IFormFile file, ImageFileOptions? options = null);

    /// <summary>
    /// Uploads multiple files to the storage service
    /// </summary>
    /// <param name="files">Collection of files to upload</param>
    /// <param name="options">File upload configuration options</param>
    /// <returns>List of identifiers of the uploaded files</returns>
    Task<Result<IList<string>>> UploadImagesAsync(IEnumerable<IFormFile> files, ImageFileOptions? options = null);

    /// <summary>
    /// Deletes a file from the storage service
    /// </summary>
    /// <param name="fileId">The identifier or URL of the file to delete</param>
    /// <returns>True if deletion was successful, false otherwise</returns>
    Task<Result<bool>> DeleteFileAsync(string fileId);
}