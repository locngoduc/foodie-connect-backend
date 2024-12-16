namespace foodie_connect_backend.Extensions.DI.EmailTemplateReader;

public interface IEmailTemplateReader
{
    Task<string> ReadTemplateAsync(string templateName);
}