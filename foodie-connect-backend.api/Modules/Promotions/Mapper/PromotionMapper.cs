using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Promotions.Dtos;

namespace foodie_connect_backend.Modules.Promotions.Mapper;

public static class PromotionMapper
{
    public static PromotionResponseDto ToResponseDto(this Promotion dto)
    {
        return new PromotionResponseDto
        {
            PromotionId = dto.Id,
            RestaurantId = dto.RestaurantId,
            Name = dto.Name,
            Description = dto.Description,
            BannerId = dto.BannerId,
            Targets = dto.Targets.ToList(),
            BeginsAt = dto.BeginsAt,
            EndsAt = dto.EndsAt,
            PromotionDetails = dto.PromotionDetails.Select(x => x.ToMinimalResponseDto()).ToArray()
        };
    }

    public static PromotionDetailsMinimalResponseDto ToMinimalResponseDto(this PromotionDetail promotionDetail)
    {
        return new PromotionDetailsMinimalResponseDto
        {
            DishId = promotionDetail.DishId,
            PromotionalPrice = promotionDetail.PromotionalPrice
        };
    }
    
    public static PromotionDetailsFullResponseDto ToFullResponseDto(this PromotionDetail promotionDetail)
    {
        return new PromotionDetailsFullResponseDto
        {
            Id = promotionDetail.PromotionId,
            Name = promotionDetail.Promotion.Name,
            Description = promotionDetail.Promotion.Description,
            BannerId = promotionDetail.Promotion.BannerId,
            Targets = promotionDetail.Promotion.Targets,
            PromotionalPrice = promotionDetail.PromotionalPrice,
            BeginsAt = promotionDetail.Promotion.BeginsAt,
            EndsAt = promotionDetail.Promotion.EndsAt
        };
    }
}