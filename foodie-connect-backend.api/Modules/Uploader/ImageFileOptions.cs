namespace foodie_connect_backend.Modules.Uploader;

public record ImageFileOptions()
{
    public string? PublicId { get; set; }
    public string? Format { get; set; }
    public string? Folder { get; set; }
};