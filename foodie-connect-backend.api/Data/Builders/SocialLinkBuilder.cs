using System;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Data.Builders;

public class SocialLinkBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private Guid _restaurantId;
    private Restaurant _restaurant = null!;
    private SocialPlatformType _platformType;
    private string _url = string.Empty;

    public SocialLinkBuilder WithId(string id) { _id = id; return this; }
    public SocialLinkBuilder WithRestaurantId(Guid restaurantId) { _restaurantId = restaurantId; return this; }
    public SocialLinkBuilder WithRestaurant(Restaurant restaurant) { _restaurant = restaurant; return this; }
    public SocialLinkBuilder WithPlatformType(SocialPlatformType type) { _platformType = type; return this; }
    public SocialLinkBuilder WithUrl(string url) { _url = url; return this; }
    public SocialLink Build() => new SocialLink
    {
        Id = _id,
        RestaurantId = _restaurantId,
        Restaurant = _restaurant,
        PlatformType = _platformType,
        Url = _url
    };
}
