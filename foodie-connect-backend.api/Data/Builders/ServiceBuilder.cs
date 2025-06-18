using System;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class ServiceBuilder: IBuilder<Service>
{
    private string _name = string.Empty;
    private bool _isDeleted = false;
    private DateTime _createdDate = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;
    private Guid _restaurantId;
    private Restaurant? _restaurant;

    public ServiceBuilder WithName(string name) { _name = name; return this; }
    public ServiceBuilder WithIsDeleted(bool isDeleted) { _isDeleted = isDeleted; return this; }
    public ServiceBuilder WithCreatedDate(DateTime createdDate) { _createdDate = createdDate; return this; }
    public ServiceBuilder WithUpdatedAt(DateTime updatedAt) { _updatedAt = updatedAt; return this; }
    public ServiceBuilder WithRestaurantId(Guid restaurantId) { _restaurantId = restaurantId; return this; }
    public ServiceBuilder WithRestaurant(Restaurant? restaurant) { _restaurant = restaurant; return this; }
    public Service Build() => new Service
    {
        Name = _name,
        IsDeleted = _isDeleted,
        CreatedDate = _createdDate,
        UpdatedAt = _updatedAt,
        RestaurantId = _restaurantId,
        Restaurant = _restaurant
    };
}
