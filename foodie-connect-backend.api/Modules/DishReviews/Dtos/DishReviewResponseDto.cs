using foodie_connect_backend.Modules.Users.Dtos;

namespace foodie_connect_backend.Modules.DishReviews.Dtos;

public class DishReviewResponseDto
{
    public Guid ReviewId { get; set; }
    public Guid DishId { get; set; }
    public UserResponseDto Author { get; set; } = null!;
    public int Rating { get; set; }
    public string Content { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}