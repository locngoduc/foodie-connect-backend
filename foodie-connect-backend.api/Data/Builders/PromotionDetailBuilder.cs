using System;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class PromotionDetailBuilder
{
    private Promotion _promotion = null!;
    private Guid _promotionId;
    private Guid _dishId;
    private decimal _promotionalPrice;

    public PromotionDetailBuilder WithPromotion(Promotion promotion) { _promotion = promotion; return this; }
    public PromotionDetailBuilder WithPromotionId(Guid id) { _promotionId = id; return this; }
    public PromotionDetailBuilder WithDishId(Guid id) { _dishId = id; return this; }
    public PromotionDetailBuilder WithPromotionalPrice(decimal price) { _promotionalPrice = price; return this; }
    public PromotionDetail Build() => new PromotionDetail
    {
        Promotion = _promotion,
        PromotionId = _promotionId,
        DishId = _dishId,
        PromotionalPrice = _promotionalPrice
    };
}
