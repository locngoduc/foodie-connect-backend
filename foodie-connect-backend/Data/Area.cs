using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public class Area
{
    public int Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
}
