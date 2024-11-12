using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using foodie_connect_backend.Shared.Dtos;

namespace foodie_connect_backend.Modules.Restaurants.Mappers;

public static class RestaurantMapper
{
    public static RestaurantResponseDto ToResponseDto(this Restaurant restaurant, ScoreResponseDto score)
    {
        return new RestaurantResponseDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            OpenTime = restaurant.OpenTime,
            CloseTime = restaurant.CloseTime,
            Status = restaurant.Status,
            SocialLinks = restaurant.SocialLinks,
            Phone = restaurant.Phone,
            Images = restaurant.Images,
            CreatedAt = restaurant.CreatedAt,
            FormattedAddress = restaurant.Area?.FormattedAddress,
            Latitude = restaurant.Location.Y,
            Longitude = restaurant.Location.X,
            HeadId = restaurant.HeadId,
            ScoreOverview = score
        };
    }
}