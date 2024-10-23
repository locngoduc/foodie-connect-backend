using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public sealed class Service
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [MaxLength(64)]
    public string Name { get; set; } = String.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? RestaurantId { get; set; }

    public Restaurant? Restaurant { get; set; }
}
