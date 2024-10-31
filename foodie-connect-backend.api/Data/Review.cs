using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Data;

public class Review
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required] [MaxLength(128)] public string Content { get; set; } = null!;
    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ReviewType Type { get; set; }
    public string? DishId { get; set; }
    public string? RestaurantId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public virtual Dish? Dish { get; set; }
    public virtual Restaurant? Restaurant { get; set; }
}