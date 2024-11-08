using System.Text.Json.Serialization;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Data;

public record SocialLink
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Guid RestaurantId { get; set; }
    [JsonIgnore]
    public Restaurant Restaurant { get; set; } = null!;

    public SocialPlatformType PlatformType { get; init; }
    public string Url { get; init; } = String.Empty;
}