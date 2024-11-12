using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public class RestaurantReview
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }

    public User User { get; set; } = null!;
    [MaxLength(128)] public string UserId { get; set; } = null!;

    [Range(1, 5)] public int Rating { get; set; } = 1;
    [MaxLength(512)] public string Content { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}