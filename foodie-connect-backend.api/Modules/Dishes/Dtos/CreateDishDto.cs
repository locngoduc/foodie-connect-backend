using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Data;

namespace foodie_connect_backend.Modules.Dishes.Dtos;

/// <summary>
/// Data transfer object for creating a new dish
/// </summary>
public abstract class CreateDishDto
{
    /// <summary>
    /// Name of the dish
    /// </summary>
    /// <example>Spicy Thai Basil Chicken</example>
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
    [MaxLength(32, ErrorMessage = "Name cannot exceed 32 characters")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Description of the dish
    /// </summary>
    /// <example>A classic Thai street food dish with minced chicken, Thai basil, and chilies</example>
    [Required(ErrorMessage = "Description is required")]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters long")]
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = null!;

    /// <summary>
    /// Price of the dish in the restaurant's currency
    /// </summary>
    /// <example>12.99</example>
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 99999999.99, ErrorMessage = "Price must be between 0.01 and 99999999.99")]
    public decimal Price { get; set; }

    /// <summary>
    /// ID of the restaurant this dish belongs to
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    [Required(ErrorMessage = "Restaurant ID is required")]
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// List of category IDs this dish belongs to
    /// </summary>
    /// <example>["1a2b3c4d", "5e6f7g8h"]</example>
    [Required(ErrorMessage = "At least one category is required")]
    public List<string> CategoryIds { get; set; } = new();
}

/// <summary>
/// Extensions for mapping between DTOs and domain models
/// </summary>
public static class DishDtoExtensions
{
    /// <summary>
    /// Maps a CreateDishDto to a Dish entity
    /// </summary>
    public static Dish ToDish(this CreateDishDto dto)
    {
        return new Dish
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            RestaurantId = dto.RestaurantId,
            // ImageId will be set after upload
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}