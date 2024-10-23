using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public partial class Category
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(16)]
    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<DishesCategory> DishesCategories { get; set; } = new List<DishesCategory>();
}
