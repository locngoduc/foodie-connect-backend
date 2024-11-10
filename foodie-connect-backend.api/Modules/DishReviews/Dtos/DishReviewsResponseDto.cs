namespace foodie_connect_backend.Modules.DishReviews.Dtos;

public class DishReviewsResponseDto
{
    public DishReviewResponseDto? MyReview { get; set; }
    public IList<DishReviewResponseDto> OtherReviews { get; set; } = new List<DishReviewResponseDto>();
}