namespace foodie_connect_backend.Modules.Promotions.Dtos;

public class PromotionDetailsFullResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? BannerId { get; set; }
    public IList<string> Targets { get; set; } = null!;
    public decimal PromotionalPrice { get; set; }
}