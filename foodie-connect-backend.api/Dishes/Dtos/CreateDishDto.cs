using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Dishes.Dtos;

public record CreateDishDto
{
    [Required] public string Name { get; set; }

    [MinLength(1)] public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    [Required] public string RestaurantId { get; set; }
}