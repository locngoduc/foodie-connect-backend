using System;
using System.Collections.Generic;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class DishCategoryBuilder
{
    private Guid _restaurantId;
    private string _categoryName = null!;
    private ICollection<Dish> _dishes = new List<Dish>();
    public DishCategoryBuilder WithRestaurantId(Guid id) { _restaurantId = id; return this; }
    public DishCategoryBuilder WithCategoryName(string name) { _categoryName = name; return this; }
    public DishCategoryBuilder WithDishes(ICollection<Dish> dishes) { _dishes = dishes; return this; }
    public DishCategory Build() => new DishCategory
    {
        RestaurantId = _restaurantId,
        CategoryName = _categoryName,
        Dishes = _dishes
    };
}
