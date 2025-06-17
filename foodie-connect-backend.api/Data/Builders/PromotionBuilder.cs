using System;
using System.Collections.Generic;
using foodie_connect_backend.Data;

namespace foodie_connect_backend.Data.Builders;

public class PromotionBuilder
{
    private readonly Promotion _promotion = new();

    public PromotionBuilder WithId(Guid id) { _promotion.Id = id; return this; }
    public PromotionBuilder WithRestaurantId(Guid restaurantId) { _promotion.RestaurantId = restaurantId; return this; }
    public PromotionBuilder WithName(string name) { _promotion.Name = name; return this; }
    public PromotionBuilder WithBannerId(string? bannerId) { _promotion.BannerId = bannerId; return this; }
    public PromotionBuilder WithDescription(string description) { _promotion.Description = description; return this; }
    public PromotionBuilder WithTargets(IList<string> targets) { _promotion.Targets = targets; return this; }
    public PromotionBuilder WithPromotionDetails(ICollection<PromotionDetail> details) { _promotion.PromotionDetails = details; return this; }
    public PromotionBuilder WithBeginsAt(DateTime beginsAt) { _promotion.BeginsAt = beginsAt; return this; }
    public PromotionBuilder WithEndsAt(DateTime endsAt) { _promotion.EndsAt = endsAt; return this; }
    public Promotion Build() => _promotion;
}
