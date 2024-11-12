using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Modules.RestaurantReviews.Dtos;

public class CreateRestaurantReviewDto
{
    [Range(1, 5)] public int Rating { get; set; } = 1;
    [MaxLength(512)] public string Content { get; set; } = null!;
}