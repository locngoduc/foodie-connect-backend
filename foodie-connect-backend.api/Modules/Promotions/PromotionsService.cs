using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Promotions.Dtos;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;

namespace foodie_connect_backend.Modules.Promotions;

public class PromotionsService(ApplicationDbContext dbContext, IUploaderService uploaderService)
{
    public async Task<Result<Promotion>> CreatePromotion(CreatePromotionDto promotion)
    {
        try
        {
            var newPromotion = new Promotion
            {
                Name = promotion.Name,
                Target = promotion.Target,
                ExpiredAt = promotion.ExpiredAt,
                DishId = promotion.DishId,
                RestaurantId = promotion.RestaurantId
            };

            await dbContext.Promotions.AddAsync(newPromotion);
            await dbContext.SaveChangesAsync();

            return Result<Promotion>.Success(newPromotion);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Result<Promotion>.Failure(AppError.InternalError());
        }
    }

    public async Task<Result<Promotion>> GetPromotionById(string id)
    {
        var promotion = await dbContext.Promotions.FindAsync(id);

        if (promotion == null)
            return Result<Promotion>.Failure(PromotionError.PromotionNotFound(id));
        return Result<Promotion>.Success(promotion);
    }

    public async Task<Result<Promotion>> UpdatePromotion(string promotionId, CreatePromotionDto promotion)
    {
        try
        {
            var result = await GetPromotionById(promotionId);
            if (result.IsFailure) return result;
            var currPromotion = result.Value;
            currPromotion.Name = promotion.Name;
            currPromotion.Target = promotion.Target;
            currPromotion.ExpiredAt = promotion.ExpiredAt;

            await dbContext.SaveChangesAsync();
            return Result<Promotion>.Success(currPromotion);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Result<Promotion>.Failure(AppError.InternalError());
        }
    }

    public async Task<Result<bool>> DeletePromotion(string promotionId)
    {
        try
        {
            var result = await GetPromotionById(promotionId);
            if (result.IsFailure) return Result<bool>.Failure(result.Error);
            var currPromotion = result.Value;
            currPromotion.IsDeleted = true;

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Result<bool>.Failure(AppError.InternalError());
        }
    }
}