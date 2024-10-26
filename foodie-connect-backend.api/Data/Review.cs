using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public class Review
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required][MaxLength(128)] public string Content { get; set; } = null!;

    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? DishId { get; set; }

    public virtual Dish? Dish { get; set; }
}
