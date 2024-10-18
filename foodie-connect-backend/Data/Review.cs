using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public class Review
{
    public int Id { get; set; }

    [Required] [MaxLength(128)] public string Content { get; set; } = null!;

    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int? DishId { get; set; }

    public virtual Dish? Dish { get; set; }
}
