using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public sealed class Service
{
    public Guid Id { get; set; }

    [MaxLength(128)] public string Name { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid RestaurantId { get; set; }

    public Restaurant? Restaurant { get; set; }
}