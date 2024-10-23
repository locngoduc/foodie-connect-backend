using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public class Promotion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(32)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(64)]
    public string Target { get; set; } = null!;

    [MaxLength(256)]
    public string BannerUrl { get; set; } = String.Empty;

    public DateTime ExpireAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? RestaurantId { get; set; }

    public string? DishId { get; set; }

    public virtual Dish? Dish { get; set; }

    public virtual Restaurant? Restaurant { get; set; }
}
