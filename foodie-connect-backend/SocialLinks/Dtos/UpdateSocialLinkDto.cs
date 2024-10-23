

using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.SocialLinks.Dtos;

public record UpdateSocialLinkDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public SocialMediaPlatform Platform { get; set; }

    [Required]
    [Url]
    public string Url { get; set; } = string.Empty;
}