using foodie_connect_backend.Modules.Users.Dtos;

namespace foodie_connect_backend.Modules.RestaurantReviews.Dtos;

public class RestaurantReviewResponseDto
{
    public Guid ReviewId { get; set; }
    public UserResponseDto Author { get; set; } = null!;
    public int Rating { get; set; }
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}