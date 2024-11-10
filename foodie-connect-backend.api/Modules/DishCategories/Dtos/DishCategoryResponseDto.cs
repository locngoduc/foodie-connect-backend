namespace foodie_connect_backend.Modules.Restaurants.Dtos;

public class DishCategoryResponseDto
{
    public Guid RestaurantId { get; init; }
    public string CategoryName { get; init; } = null!;
}