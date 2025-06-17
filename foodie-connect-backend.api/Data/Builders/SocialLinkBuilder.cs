using System;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Enums;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class SocialLinkBuilder: IBuilder<SocialLink>
{
    private Guid _restaurantId;
    private Restaurant _restaurant = null!;
    private SocialPlatformType _platformType;
    private string _url = string.Empty;

    public SocialLinkBuilder WithRestaurantId(Guid restaurantId) { _restaurantId = restaurantId; return this; }
    public SocialLinkBuilder WithRestaurant(Restaurant restaurant) { _restaurant = restaurant; return this; }
    public SocialLinkBuilder WithPlatformType(SocialPlatformType type) { _platformType = type; return this; }
    public SocialLinkBuilder WithUrl(string url) { _url = url; return this; }
    public SocialLink Build() => new SocialLink
    {
        RestaurantId = _restaurantId,
        Restaurant = _restaurant,
        PlatformType = _platformType,
        Url = _url
    };
}
