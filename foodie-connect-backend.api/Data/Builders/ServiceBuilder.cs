using System;
using foodie_connect_backend.Data;

namespace foodie_connect_backend.Data.Builders;

public class ServiceBuilder
{
    private readonly Service _service = new();

    public ServiceBuilder WithId(Guid id) { _service.Id = id; return this; }
    public ServiceBuilder WithName(string name) { _service.Name = name; return this; }
    public ServiceBuilder WithIsDeleted(bool isDeleted) { _service.IsDeleted = isDeleted; return this; }
    public ServiceBuilder WithCreatedDate(DateTime createdDate) { _service.CreatedDate = createdDate; return this; }
    public ServiceBuilder WithUpdatedAt(DateTime updatedAt) { _service.UpdatedAt = updatedAt; return this; }
    public ServiceBuilder WithRestaurantId(Guid restaurantId) { _service.RestaurantId = restaurantId; return this; }
    public ServiceBuilder WithRestaurant(Restaurant? restaurant) { _service.Restaurant = restaurant; return this; }
    public Service Build() => _service;
}
