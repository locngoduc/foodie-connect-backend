using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Modules.DishReviews.Dtos;

public class UpdateDishReviewDto
{
    [Range(1, 5)] public int Rating { get; set; }
    public string Content { get; set; } = null!;
}