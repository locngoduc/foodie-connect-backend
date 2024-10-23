
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.SocialLinks.Dtos;
public class SocialLinkResponseDto
{
    public string Id { get; set; } = string.Empty;
    public SocialMediaPlatform Platform { get; set; }
    public string Url { get; set; } = string.Empty;
}