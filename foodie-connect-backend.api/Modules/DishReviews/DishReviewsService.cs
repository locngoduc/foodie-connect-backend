using foodie_connect_backend.Data;
using foodie_connect_backend.Data.Builders;
using foodie_connect_backend.Modules.DishReviews.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Modules.DishReviews;

public class DishReviewsService(ApplicationDbContext dbContext)
{
    public async Task<Result<DishReview>> AddReview(Guid dishId, CreateDishReviewDto dto, string reviewerId)
    {
        var dish = await dbContext.Dishes
            .Include(dish => dish.Reviews)
            .FirstOrDefaultAsync(dish => dish.Id == dishId);
        if (dish is null) return Result<DishReview>.Failure(DishError.DishNotFound());
        
        // User can only add one review per dish
        var timesReviewed = dish.Reviews.Count(review => review.UserId == reviewerId);
        if (timesReviewed != 0) return Result<DishReview>.Failure(DishError.AlreadyReviewed());

        // Add review to database
        var review = new DishReview
        {
            DishId = dish.Id,
            UserId = reviewerId,
            Rating = dto.Rating,
            Content = dto.Content,
        };
        
        dbContext.DishReviews.Add(review);
        await dbContext.SaveChangesAsync();
        
        var addedReview = await dbContext.DishReviews
            .Include(dishReview => dishReview.User)
            .FirstOrDefaultAsync(dishReview => dishReview.Id == review.Id);
        
        return Result<DishReview>.Success(addedReview!);
    }

    public async Task<Result<IEnumerable<DishReview>>> GetReviews(Guid dishId)
    {
        var dishReviews = await dbContext.DishReviews
            .Include(dishReview => dishReview.User)
            .Where(dishReview => dishReview.DishId == dishId)
            .ToListAsync();
        
        return Result<IEnumerable<DishReview>>.Success(dishReviews);
    }

    public async Task<Result<DishReview>> UpdateReview(string requesterId, Guid reviewId, UpdateDishReviewDto dto)
    {
        var dishReview = await dbContext.DishReviews
            .Include(dishReview => dishReview.User)
            .FirstOrDefaultAsync(dishReview => dishReview.Id == reviewId);
        
        if (dishReview is null) return Result<DishReview>.Failure(DishError.ReviewNotFound());
        if (dishReview.UserId != requesterId) return Result<DishReview>.Failure(AuthError.NotAuthorized());
        
        dishReview.Rating = dto.Rating;
        dishReview.Content = dto.Content;
        dishReview.UpdatedAt = DateTime.Now;
        
        await dbContext.SaveChangesAsync();
        return Result<DishReview>.Success(dishReview);
    }
    
    public async Task<Result<DishReview>> DeleteReview(string requesterId, Guid reviewId)
    {
        var dishReview = await dbContext.DishReviews
            .Include(dishReview => dishReview.User)
            .FirstOrDefaultAsync(dishReview => dishReview.Id == reviewId);
        
        if (dishReview is null) return Result<DishReview>.Failure(DishError.ReviewNotFound());
        if (dishReview.UserId != requesterId) return Result<DishReview>.Failure(AuthError.NotAuthorized());
        
        dbContext.DishReviews.Remove(dishReview);
        await dbContext.SaveChangesAsync();
        return Result<DishReview>.Success(dishReview);
    }
}