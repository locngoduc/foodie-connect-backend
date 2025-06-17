using System;
using System.Collections.Generic;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class PromotionBuilder
{
    private Guid _id;
    private Guid _restaurantId;
    private string _name = null!;
    private string? _bannerId;
    private string _description = null!;
    private IList<string> _targets = new List<string>();
    private ICollection<PromotionDetail> _promotionDetails = new List<PromotionDetail>();
    private DateTime _beginsAt;
    private DateTime _endsAt;

    public PromotionBuilder WithId(Guid id) { _id = id; return this; }
    public PromotionBuilder WithRestaurantId(Guid restaurantId) { _restaurantId = restaurantId; return this; }
    public PromotionBuilder WithName(string name) { _name = name; return this; }
    public PromotionBuilder WithBannerId(string? bannerId) { _bannerId = bannerId; return this; }
    public PromotionBuilder WithDescription(string description) { _description = description; return this; }
    public PromotionBuilder WithTargets(IList<string> targets) { _targets = targets; return this; }
    public PromotionBuilder WithPromotionDetails(ICollection<PromotionDetail> details) { _promotionDetails = details; return this; }
    public PromotionBuilder WithBeginsAt(DateTime beginsAt) { _beginsAt = beginsAt; return this; }
    public PromotionBuilder WithEndsAt(DateTime endsAt) { _endsAt = endsAt; return this; }
    public Promotion Build() => new Promotion
    {
        Id = _id,
        RestaurantId = _restaurantId,
        Name = _name,
        BannerId = _bannerId,
        Description = _description,
        Targets = _targets,
        PromotionDetails = _promotionDetails,
        BeginsAt = _beginsAt,
        EndsAt = _endsAt
    };
}
