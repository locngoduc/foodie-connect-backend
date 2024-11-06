using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Modules.Socials.Dtos;

public record UpdateSocialDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public SocialPlatformType PlatformType { get; set; }

    [Required]
    [Url]
    public string Url { get; set; } = string.Empty;
}