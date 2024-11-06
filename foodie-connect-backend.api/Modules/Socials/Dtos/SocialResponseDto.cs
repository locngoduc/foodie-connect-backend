
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Modules.Socials.Dtos;
public record SocialLinkResponseDto
{
    public string Id { get; set; } = string.Empty;
    public SocialPlatformType PlatformType { get; set; }
    public string Url { get; set; } = string.Empty;
}