using System.Text.Json.Serialization;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Shared.Classes;

public record SocialLink
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string? RestaurantId { get; set; }
    [JsonIgnore]
    public Restaurant Restaurant { get; set; } = null!;

    public SocialMediaPlatform Platform { get; init; }
    public string Url { get; init; } = String.Empty;
}