using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public sealed class Service
{
    public int Id { get; set; }

    [MaxLength(64)]
    public string Name { get; set; } = String.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int? RestaurantId { get; set; }

    public Restaurant? Restaurant { get; set; }
}
