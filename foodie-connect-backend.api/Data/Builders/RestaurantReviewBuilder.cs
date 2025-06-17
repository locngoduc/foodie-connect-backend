using System;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class RestaurantReviewBuilder
{
    private Guid _id;
    private Guid _restaurantId;
    private User _user = null!;
    private string _userId = null!;
    private int _rating = 1;
    private string _content = null!;
    private DateTime _createdAt = DateTime.Now;
    private DateTime _updatedAt = DateTime.Now;
    public RestaurantReviewBuilder WithId(Guid id) { _id = id; return this; }
    public RestaurantReviewBuilder WithRestaurantId(Guid restaurantId) { _restaurantId = restaurantId; return this; }
    public RestaurantReviewBuilder WithUser(User user) { _user = user; return this; }
    public RestaurantReviewBuilder WithUserId(string userId) { _userId = userId; return this; }
    public RestaurantReviewBuilder WithRating(int rating) { _rating = rating; return this; }
    public RestaurantReviewBuilder WithContent(string content) { _content = content; return this; }
    public RestaurantReviewBuilder WithCreatedAt(DateTime createdAt) { _createdAt = createdAt; return this; }
    public RestaurantReviewBuilder WithUpdatedAt(DateTime updatedAt) { _updatedAt = updatedAt; return this; }
    public RestaurantReview Build() => new RestaurantReview
    {
        Id = _id,
        RestaurantId = _restaurantId,
        User = _user,
        UserId = _userId,
        Rating = _rating,
        Content = _content,
        CreatedAt = _createdAt,
        UpdatedAt = _updatedAt
    };
}
