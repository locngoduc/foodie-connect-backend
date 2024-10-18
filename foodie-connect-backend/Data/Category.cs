using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public partial class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(16)]
    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<DishesCategory> DishesCategories { get; set; } = new List<DishesCategory>();
}
