using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.DishReviews.Dtos;
using foodie_connect_backend.Modules.Users.Dtos;

namespace foodie_connect_backend.Modules.DishReviews.Mapper;

public static class DishReviewMapper
{
    public static DishReviewResponseDto ToResponseDto(this DishReview dishReview)
    {
        return new DishReviewResponseDto
        {
            ReviewId = dishReview.Id,
            DishId = dishReview.DishId,
            Author = new UserResponseDto {
                Id = dishReview.UserId,
                DisplayName = dishReview.User.DisplayName,
                UserName = dishReview.User.UserName
            },
            Rating = dishReview.Rating,
            Content = dishReview.Content,
            CreatedAt = dishReview.CreatedAt,
            UpdatedAt = dishReview.UpdatedAt,
        };
    }
}