using foodie_connect_backend.Modules.DishReviews.Dtos;

namespace foodie_connect_backend.Modules.Dishes.Dtos;

public class DishResponseDto
{
    public Guid DishId { get; init; }
    public Guid RestaurantId { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string? ImageId { get; init; }
    public decimal Price { get; init; } = 0;
    public string[] Categories { get; init; } = [];
    public DishScoreResponseDto ScoreOverview { get; init; } = null!;
}