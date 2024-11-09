using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public sealed class Dish
{
    public Guid Id { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(32)]
    public string Name { get; set; } = null!;

    [MaxLength(256)]
    public string? ImageId { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid RestaurantId { get; init; }
    
    public Restaurant Restaurant { get; init; } = null!;
    
    public ICollection<DishCategory> Categories { get; set; } = new List<DishCategory>();

    public bool IsDeleted { get; init; }

    public ICollection<Promotion> Promotions { get; init; } = new List<Promotion>();
    
    public ICollection<DishReview> Reviews { get; init; } = new List<DishReview>();
}