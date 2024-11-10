using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Modules.DishReviews.Dtos;

namespace foodie_connect_backend.Modules.Dishes.Dtos;

/// <summary>
/// Represents the response data for a dish, including its details and review information
/// </summary>
public class DishResponseDto
{
    /// <summary>
    /// The unique identifier for the dish
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid DishId { get; init; }

    /// <summary>
    /// The unique identifier of the restaurant this dish belongs to
    /// </summary>
    /// <example>7c9e6679-7425-40de-944b-e07fc1f90ae7</example>
    public Guid RestaurantId { get; init; }

    /// <summary>
    /// The name of the dish
    /// </summary>
    /// <example>Spicy Tuna Roll</example>
    [Required]
    public string Name { get; init; } = null!;

    /// <summary>
    /// A detailed description of the dish
    /// </summary>
    /// <example>Fresh tuna mixed with spicy mayo, wrapped in seasoned rice and nori</example>
    [Required]
    public string Description { get; init; } = null!;

    /// <summary>
    /// The identifier for the dish's image, if one exists
    /// </summary>
    /// <example>dish_123_image</example>
    public string? ImageId { get; init; }

    /// <summary>
    /// The price of the dish in the restaurant's local currency
    /// </summary>
    /// <example>12.99</example>
    [Range(0, double.MaxValue)]
    public decimal Price { get; init; } = 0;

    /// <summary>
    /// The categories or tags associated with this dish
    /// </summary>
    /// <example>["Sushi", "Spicy", "Seafood"]</example>
    [Required]
    public string[] Categories { get; init; } = [];

    /// <summary>
    /// Overview of all review scores for this dish
    /// </summary>
    [Required]
    public DishScoreResponseDto ScoreOverview { get; init; } = null!;
}