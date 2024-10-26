namespace foodie_connect_backend.Uploader;

public record ImageFileOptions()
{
    public string? PublicId { get; set; }
    public string? Format { get; set; }
    public string? Folder { get; set; }
};