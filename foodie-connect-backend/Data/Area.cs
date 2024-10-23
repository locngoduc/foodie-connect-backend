using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public class Area
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(32)]
    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
}
