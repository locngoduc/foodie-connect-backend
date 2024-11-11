using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Promotions.Dtos;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Modules.Promotions;

public class PromotionsService(ApplicationDbContext dbContext, IUploaderService uploaderService)
{
    public async Task<Result<Promotion>> CreatePromotion(Guid restaurantId, CreatePromotionDto dto)
    {
        // Check if dishes belong to the restaurant
        var dishIds = dto.PromotionDetails.Select(x => x.DishId).ToList();
        var dishes = await dbContext.Dishes.Where(x => dishIds.Contains(x.Id)).ToListAsync();
        if (dishes.Count != dishIds.Count)
        {
            return Result<Promotion>.Failure(PromotionError.PromotionDishNotFound());
        }
        
        var promotion = new Promotion
        {
            Name = dto.Name,
            RestaurantId = restaurantId,
            Description = dto.Description,
            Targets = dto.Targets,
            BeginsAt = dto.BeginsAt,
            EndsAt = dto.EndsAt,
            PromotionDetails = dto.PromotionDetails.Select(x => new PromotionDetail
            {
                DishId = x.DishId,
                PromotionalPrice = x.PromotionalPrice
            }).ToList()
        };

        try
        {
            dbContext.Promotions.Add(promotion);
            await dbContext.SaveChangesAsync();
            return Result<Promotion>.Success(promotion);
        }
        catch
        {
            return Result<Promotion>.Failure(PromotionError.PromotionDishNotFound());
        }
    }
}