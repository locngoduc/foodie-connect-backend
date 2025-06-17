using System;
using foodie_connect_backend.Data;

namespace foodie_connect_backend.Data.Builders;

public class DishReviewBuilder
{
    private readonly DishReview _review = new();

    public DishReviewBuilder WithId(Guid id) { _review.Id = id; return this; }
    public DishReviewBuilder WithDishId(Guid dishId) { _review.DishId = dishId; return this; }
    public DishReviewBuilder WithUser(User user) { _review.User = user; return this; }
    public DishReviewBuilder WithUserId(string userId) { _review.UserId = userId; return this; }
    public DishReviewBuilder WithRating(int rating) { _review.Rating = rating; return this; }
    public DishReviewBuilder WithContent(string content) { _review.Content = content; return this; }
    public DishReviewBuilder WithCreatedAt(DateTime createdAt) { _review.CreatedAt = createdAt; return this; }
    public DishReviewBuilder WithUpdatedAt(DateTime updatedAt) { _review.UpdatedAt = updatedAt; return this; }
    public DishReview Build() => _review;
}
