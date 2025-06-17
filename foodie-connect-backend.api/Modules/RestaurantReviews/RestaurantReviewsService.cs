using foodie_connect_backend.Data;
using foodie_connect_backend.Data.Builders;
using foodie_connect_backend.Modules.RestaurantReviews.Dtos;
using foodie_connect_backend.Modules.RestaurantReviews.Mapper;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Modules.RestaurantReviews;

public class RestaurantReviewsService(ApplicationDbContext dbContext)
{
    public async Task<Result<RestaurantReview>> AddReview(Guid restaurantId, string reviewerId, CreateRestaurantReviewDto dto)
    {
        // Check if user has reviewed before
        var existingReview = await dbContext.RestaurantReviews
            .Where(r => r.RestaurantId == restaurantId && r.UserId == reviewerId)
            .FirstOrDefaultAsync();
        if (existingReview != null)
            return Result<RestaurantReview>.Failure(RestaurantError.AlreadyReviewed());
        
        // Add review
        var review = new RestaurantReviewBuilder()
            .WithRestaurantId(restaurantId)
            .WithUserId(reviewerId)
            .WithRating(dto.Rating)
            .WithContent(dto.Content)
            .Build();
        
        dbContext.RestaurantReviews.Add(review);
        await dbContext.SaveChangesAsync();
        
        // Get the review with author
        var result = await dbContext.RestaurantReviews
            .Include(r => r.User)
            .FirstAsync(r => r.Id == review.Id);

        return Result<RestaurantReview>.Success(result);
    }

    public async Task<Result<RestaurantReviewsResponseDto>> GetReviews(Guid restaurantId, string? requesterId)
    {
        var restaurant = await dbContext.Restaurants
            .Include(r => r.Reviews)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == restaurantId);
        if (restaurant == null)
            return Result<RestaurantReviewsResponseDto>.Failure(RestaurantError.RestaurantNotExist());

        RestaurantReview? myReview = null;
        if (requesterId is not null)
        {
            myReview = restaurant.Reviews.FirstOrDefault(r => r.UserId == requesterId);
        }
        
        var otherReviews = restaurant.Reviews.Where(r => r.UserId != requesterId).ToList();
        
        var response = new RestaurantReviewsResponseDto
        {
            MyReview = myReview?.ToResponseDto(),
            OtherReviews = otherReviews.Select(r => r.ToResponseDto()).ToList()
        };

        return Result<RestaurantReviewsResponseDto>.Success(response);
    }
    
    public async Task<Result<RestaurantReview>> UpdateReview(Guid restaurantId, string reviewerId, Guid reviewId, CreateRestaurantReviewDto dto)
    {
        var review = await dbContext.RestaurantReviews
            .Where(r => r.RestaurantId == restaurantId && r.Id == reviewId)
            .FirstOrDefaultAsync();
        if (review == null)
            return Result<RestaurantReview>.Failure(RestaurantError.ReviewNotExist());
        if (review.UserId != reviewerId)
            return Result<RestaurantReview>.Failure(AuthError.NotAuthorized());
        
        review.Rating = dto.Rating;
        review.Content = dto.Content;
        review.UpdatedAt = DateTime.Now;
        
        await dbContext.SaveChangesAsync();
        
        return Result<RestaurantReview>.Success(review);
    }
    
    public async Task<Result<RestaurantReview>> DeleteReview(Guid restaurantId, Guid reviewId, string reviewerId)
    {
        var review = await dbContext.RestaurantReviews
            .Where(r => r.RestaurantId == restaurantId && r.Id == reviewId)
            .FirstOrDefaultAsync();
        if (review == null)
            return Result<RestaurantReview>.Failure(RestaurantError.ReviewNotExist());
        if (review.UserId != reviewerId)
            return Result<RestaurantReview>.Failure(AuthError.NotAuthorized());
        
        dbContext.RestaurantReviews.Remove(review);
        await dbContext.SaveChangesAsync();
        
        return Result<RestaurantReview>.Success(review);
    }
}