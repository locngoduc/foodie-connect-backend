using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.RestaurantReviews.Dtos;
using foodie_connect_backend.Modules.Users.Dtos;

namespace foodie_connect_backend.Modules.RestaurantReviews.Mapper;

public static class RestaurantReviewMapper
{
    public static RestaurantReviewResponseDto ToResponseDto(this RestaurantReview restaurantReview)
    {
        return new RestaurantReviewResponseDto
        {
            ReviewId = restaurantReview.Id,
            Author = new UserResponseDto {
                Id = restaurantReview.UserId,
                DisplayName = restaurantReview.User.DisplayName,
                UserName = restaurantReview.User.UserName
            },
            Rating = restaurantReview.Rating,
            Content = restaurantReview.Content,
            CreatedAt = restaurantReview.CreatedAt,
            UpdatedAt = restaurantReview.UpdatedAt,
        };
    }
}