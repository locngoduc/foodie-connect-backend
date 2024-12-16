namespace foodie_connect_backend.Extensions.DI.EmailTemplateReader;

public class FileEmailTemplateReader(IWebHostEnvironment env) : IEmailTemplateReader
{
    private readonly string _basePath = Path.Combine(env.ContentRootPath, "Modules", "Verification", "Templates");

    public async Task<string> ReadTemplateAsync(string templateName)
    {
        var fullPath = Path.Combine(_basePath, templateName);
        return await File.ReadAllTextAsync(fullPath);
    }
}