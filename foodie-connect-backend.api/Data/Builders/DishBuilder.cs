using System;
using System.Collections.Generic;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class DishBuilder: IBuilder<Dish>
{
    private Guid _id;
    private string _name = null!;
    private string? _imageId;
    private string _description = null!;
    private decimal _price;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;
    private Guid _restaurantId;
    private Restaurant _restaurant = null!;
    private ICollection<DishCategory> _categories = new List<DishCategory>();
    private bool _isDeleted;
    private ICollection<PromotionDetail> _promotionDetails = new List<PromotionDetail>();
    private ICollection<DishReview> _reviews = new List<DishReview>();

    public DishBuilder WithId(Guid id) { _id = id; return this; }
    public DishBuilder WithName(string name) { _name = name; return this; }
    public DishBuilder WithImageId(string? imageId) { _imageId = imageId; return this; }
    public DishBuilder WithDescription(string description) { _description = description; return this; }
    public DishBuilder WithPrice(decimal price) { _price = price; return this; }
    public DishBuilder WithCreatedAt(DateTime createdAt) { _createdAt = createdAt; return this; }
    public DishBuilder WithUpdatedAt(DateTime updatedAt) { _updatedAt = updatedAt; return this; }
    public DishBuilder WithRestaurantId(Guid restaurantId) { _restaurantId = restaurantId; return this; }
    public DishBuilder WithRestaurant(Restaurant restaurant) { _restaurant = restaurant; return this; }
    public DishBuilder WithCategories(ICollection<DishCategory> categories) { _categories = categories; return this; }
    public DishBuilder WithIsDeleted(bool isDeleted) { _isDeleted = isDeleted; return this; }
    public DishBuilder WithPromotionDetails(ICollection<PromotionDetail> details) { _promotionDetails = details; return this; }
    public DishBuilder WithReviews(ICollection<DishReview> reviews) { _reviews = reviews; return this; }
    public Dish Build() => new Dish
    {
        Id = _id,
        Name = _name,
        ImageId = _imageId,
        Description = _description,
        Price = _price,
        CreatedAt = _createdAt,
        UpdatedAt = _updatedAt,
        RestaurantId = _restaurantId,
        Restaurant = _restaurant,
        Categories = _categories,
        IsDeleted = _isDeleted,
        PromotionDetails = _promotionDetails,
        Reviews = _reviews
    };
}
