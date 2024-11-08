using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public sealed class Dish
{
    public Guid Id { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(32)]
    public string Name { get; init; } = null!;

    public string? ImageId { get; init; }

    [MaxLength(500)]
    public string Description { get; init; } = null!;

    public decimal Price { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    public Guid RestaurantId { get; init; }
    
    public ICollection<DishCategory> Categories { get; init; } = new List<DishCategory>();

    public bool IsDeleted { get; init; }

    public ICollection<Promotion> Promotions { get; init; } = new List<Promotion>();

    public Restaurant? Restaurant { get; init; }

    public ICollection<DishReview> Reviews { get; init; } = new List<DishReview>();
}