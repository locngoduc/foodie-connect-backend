using foodie_connect_backend.Modules.GeoCoder;
using foodie_connect_backend.Modules.Uploader;

namespace foodie_connect_backend.Extensions.DI;

public static class RegisterThirdPartyDependencies
{
    public static void AddThirdPartyDependencies(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        
        // Mailtrap sender
        services.AddFluentEmail("verify@account.foodie.town", "Verify your email address")
            .AddMailtrapSender(
                configuration["MAILTRAP_USERNAME"],
                configuration["MAILTRAP_PASSWORD"],
                configuration["MAILTRAP_HOST"],
                int.TryParse(configuration["MAILTRAP_PORT"], out var port) ? port : null);
        
        // Google GeoCoder
        services.AddScoped<IGeoCoderService, ReverseGeoCoder>(_ =>
            new ReverseGeoCoder(configuration["GOOGLE_APIKEY"]!));
        
        // Cloudinary CDN uploader
        services.AddScoped<IUploaderService, CloudinaryUploader>();
    }
}