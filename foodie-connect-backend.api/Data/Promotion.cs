using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public class Promotion
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    [MaxLength(128)] public string Name { get; set; } = null!;
    [MaxLength(128)] public string? BannerId { get; set; }
    [MaxLength(2048)] public string Description { get; set; } = null!;
    public IList<string> Targets { get; set; } = new List<string>();
    public ICollection<PromotionDetail> PromotionDetails { get; set; } = new List<PromotionDetail>();
    public DateTime BeginsAt { get; set; }
    public DateTime EndsAt { get; set; }
}