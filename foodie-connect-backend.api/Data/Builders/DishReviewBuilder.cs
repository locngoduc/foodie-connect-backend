using System;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class DishReviewBuilder
{
    private Guid _dishId;
    private User _user = null!;
    private string _userId = null!;
    private int _rating = 1;
    private string _content = null!;
    private DateTime _createdAt = DateTime.Now;
    private DateTime _updatedAt = DateTime.Now;
    public DishReviewBuilder WithDishId(Guid dishId) { _dishId = dishId; return this; }
    public DishReviewBuilder WithUser(User user) { _user = user; return this; }
    public DishReviewBuilder WithUserId(string userId) { _userId = userId; return this; }
    public DishReviewBuilder WithRating(int rating) { _rating = rating; return this; }
    public DishReviewBuilder WithContent(string content) { _content = content; return this; }
    public DishReviewBuilder WithCreatedAt(DateTime createdAt) { _createdAt = createdAt; return this; }
    public DishReviewBuilder WithUpdatedAt(DateTime updatedAt) { _updatedAt = updatedAt; return this; }
    public DishReview Build() => new DishReview
    {
        DishId = _dishId,
        User = _user,
        UserId = _userId,
        Rating = _rating,
        Content = _content,
        CreatedAt = _createdAt,
        UpdatedAt = _updatedAt
    };
}
