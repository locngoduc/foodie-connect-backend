using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.SocialLinks.Dtos;

public record CreateSocialLinkDto
{
    [Required]
    [EnumDataType(typeof(SocialMediaPlatform))]
    public SocialMediaPlatform Platform { get; set; }

    [Required] [Url] public string Url { get; set; } = string.Empty;
}