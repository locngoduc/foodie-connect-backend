using System;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class PromotionDetailBuilder: IBuilder<PromotionDetail>
{
    private readonly PromotionDetail _detail = new();

    public PromotionDetailBuilder WithPromotion(Promotion promotion) { _detail.Promotion = promotion; return this; }
    public PromotionDetailBuilder WithPromotionId(Guid id) { _detail.PromotionId = id; return this; }
    public PromotionDetailBuilder WithDishId(Guid id) { _detail.DishId = id; return this; }
    public PromotionDetailBuilder WithPromotionalPrice(decimal price) { _detail.PromotionalPrice = price; return this; }
    public PromotionDetail Build() => _detail;
}
