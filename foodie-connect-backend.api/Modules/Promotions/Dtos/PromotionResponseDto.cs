namespace foodie_connect_backend.Modules.Promotions.Dtos;

public class PromotionResponseDto
{
    public Guid PromotionId { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = null!;
    public string? BannerId { get; set; }
    public string Description { get; set; } = null!;
    public IList<string> Targets { get; set; } = new List<string>();
    public DateTime BeginsAt { get; set; }
    public DateTime EndsAt { get; set; }
    
    public ICollection<PromotionDetailsMinimalResponseDto> PromotionDetails { get; set; } =  null!;
}