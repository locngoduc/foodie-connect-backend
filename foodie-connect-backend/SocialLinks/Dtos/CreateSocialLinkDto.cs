using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.SocialLinks.Dtos;

public record CreateSocialLinkDto
{
    [Required(ErrorMessage = "The Platform field is required.")]
    [EnumDataType(typeof(SocialMediaPlatform), ErrorMessage = "The selected platform is invalid.")]
    public SocialMediaPlatform Platform { get; set; }

    [Required(ErrorMessage = "The Url field is required.")]
    [Url(ErrorMessage = "The Url field is not a valid fully-qualified http, https, or ftp URL.")]
    public string Url { get; set; } = string.Empty;
}
