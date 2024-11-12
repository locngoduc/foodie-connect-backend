namespace foodie_connect_backend.Modules.RestaurantReviews.Dtos;

public class RestaurantReviewsResponseDto
{
    public RestaurantReviewResponseDto? MyReview { get; set; }
    public IList<RestaurantReviewResponseDto> OtherReviews { get; set; } = new List<RestaurantReviewResponseDto>();
}