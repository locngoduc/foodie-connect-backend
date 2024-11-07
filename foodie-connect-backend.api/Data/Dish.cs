using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public sealed class Dish
{
    [MaxLength(128)]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [Required]
    [MinLength(3)]
    [MaxLength(32)]
    public string Name { get; init; } = null!;

    public string ImageId { get; init; } = null!;

    [MaxLength(500)]
    public string Description { get; init; } = null!;

    public decimal Price { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    [MaxLength(128)]
    public string RestaurantId { get; init; } = null!;
    
    public ICollection<DishCategory> Categories { get; init; } = new List<DishCategory>();

    public bool IsDeleted { get; init; }

    public ICollection<Promotion> Promotions { get; init; } = new List<Promotion>();

    public Restaurant? Restaurant { get; init; }

    public ICollection<Review> Reviews { get; init; } = new List<Review>();
}