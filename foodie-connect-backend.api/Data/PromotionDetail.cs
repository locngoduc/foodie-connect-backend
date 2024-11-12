using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Data;

[PrimaryKey(nameof(PromotionId), nameof(DishId))]
public class PromotionDetail
{
    public Promotion Promotion { get; set; } = null!;
    public Guid PromotionId { get; set; }
    public Guid DishId { get; set; }
    public decimal PromotionalPrice { get; set; }
}