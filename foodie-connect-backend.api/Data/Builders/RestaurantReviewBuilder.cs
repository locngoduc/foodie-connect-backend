using System;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class RestaurantReviewBuilder: IBuilder<RestaurantReview>
{
    private readonly RestaurantReview _review = new();

    public RestaurantReviewBuilder WithId(Guid id) { _review.Id = id; return this; }
    public RestaurantReviewBuilder WithRestaurantId(Guid restaurantId) { _review.RestaurantId = restaurantId; return this; }
    public RestaurantReviewBuilder WithUser(User user) { _review.User = user; return this; }
    public RestaurantReviewBuilder WithUserId(string userId) { _review.UserId = userId; return this; }
    public RestaurantReviewBuilder WithRating(int rating) { _review.Rating = rating; return this; }
    public RestaurantReviewBuilder WithContent(string content) { _review.Content = content; return this; }
    public RestaurantReviewBuilder WithCreatedAt(DateTime createdAt) { _review.CreatedAt = createdAt; return this; }
    public RestaurantReviewBuilder WithUpdatedAt(DateTime updatedAt) { _review.UpdatedAt = updatedAt; return this; }
    public RestaurantReview Build() => _review;
}
