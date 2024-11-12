namespace foodie_connect_backend.Modules.Promotions.Dtos;

public class PromotionDetailsMinimalResponseDto
{
    /// <summary>
    /// The unique identifier for the dish
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid DishId { get; set; }
    
    /// <summary>
    /// The price of the dish in the restaurant's local currency
    /// </summary>
    /// <example>12.99</example>
    public decimal PromotionalPrice { get; set; }
}