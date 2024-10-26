using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public partial class Dish
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MinLength(3)]
    [MaxLength(32)]
    public string Name { get; set; } = null!;

    [MaxLength(256)]
    public string ImageId { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? RestaurantId { get; set; }

    public virtual ICollection<DishesCategory> DishesCategories { get; set; } = new List<DishesCategory>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual Restaurant? Restaurant { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
