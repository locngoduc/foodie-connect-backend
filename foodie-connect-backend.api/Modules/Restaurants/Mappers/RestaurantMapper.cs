using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Restaurants.Dtos;

namespace foodie_connect_backend.Modules.Restaurants.Mappers;

public static class RestaurantMapper
{
    public static DishCategoryResponseDto ToResponseDto(this DishCategory dishCategory)
    {
        return new DishCategoryResponseDto()
        {
            RestaurantId = dishCategory.RestaurantId,
            CategoryName = dishCategory.CategoryName,
        };
    }
}