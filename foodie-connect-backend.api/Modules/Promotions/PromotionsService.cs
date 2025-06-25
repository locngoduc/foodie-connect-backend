using foodie_connect_backend.Data;
using foodie_connect_backend.Data.Builders;
using foodie_connect_backend.Modules.Promotions.Dtos;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

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
        
        // Add promotion to DB
        var promotion = new Promotion
        {
            RestaurantId = restaurantId,
            Name = dto.Name,
            Description = dto.Description,
            Targets = dto.Targets,
            BeginsAt = dto.BeginsAt,
            EndsAt = dto.EndsAt,
        };

        await dbContext.Promotions.AddAsync(promotion);
        await dbContext.SaveChangesAsync();
        
        // Add promotion details to DB
        var promotionDetails = dto.PromotionDetails.Select(x => new PromotionDetail
        {
            PromotionId = promotion.Id,
            DishId = x.DishId,
            PromotionalPrice = x.PromotionalPrice
        });
        
        foreach (var detail in promotionDetails)
        {
            promotion.PromotionDetails.Add(detail);
        }
        
        await dbContext.SaveChangesAsync();

        var promotionWithDetails = await dbContext.Promotions
            .Include(x => x.PromotionDetails)
            .FirstOrDefaultAsync(x => x.Id == promotion.Id);
        
        return Result<Promotion>.Success(promotionWithDetails!);
    }

    public async Task<Result<Promotion>> GetPromotion(Guid promotionId, Guid restaurantId)
    {
        var promotion = await dbContext.Promotions
            .Include(x => x.PromotionDetails)
            .FirstOrDefaultAsync(x => x.Id == promotionId && x.RestaurantId == restaurantId);
        if (promotion is null)
            return Result<Promotion>.Failure(PromotionError.PromotionNotFound());

        return Result<Promotion>.Success(promotion);
    }
    
    public async Task<Result<List<Promotion>>> GetPromotions(Guid restaurantId)
    {
        var promotions = await dbContext.Promotions
            .Include(x => x.PromotionDetails)
            .Where(x => x.RestaurantId == restaurantId)
            .ToListAsync();

        return Result<List<Promotion>>.Success(promotions);
    }

    public async Task<Result<bool>> DeletePromotion(Guid promotion, Guid restaurantId)
    {
        var promotionToDelete = await dbContext.Promotions.FirstOrDefaultAsync(x => x.Id == promotion && x.RestaurantId == restaurantId);
        if (promotionToDelete is null)
            return Result<bool>.Failure(PromotionError.PromotionNotFound());
        
        dbContext.Promotions.Remove(promotionToDelete);
        await dbContext.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}