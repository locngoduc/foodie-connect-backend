namespace foodie_connect_backend.Modules.Promotions.Dtos;

public class CreatePromotionDetailsDto
{
    public Guid DishId { get; set; }
    public decimal PromotionalPrice { get; set; }
}