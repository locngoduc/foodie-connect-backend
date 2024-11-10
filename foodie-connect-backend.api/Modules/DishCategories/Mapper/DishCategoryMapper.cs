using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Restaurants.Dtos;

namespace foodie_connect_backend.Modules.DishCategories.Mapper;

public static class DishCategoryMapper
{
    public static DishCategoryResponseDto ToResponseDto(this DishCategory dishCategory)
    {
        return new DishCategoryResponseDto
        {
            RestaurantId = dishCategory.RestaurantId,
            CategoryName = dishCategory.CategoryName,
        };
    }
}