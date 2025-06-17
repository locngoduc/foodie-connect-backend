using System;
using System.Collections.Generic;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class DishCategoryBuilder: IBuilder<DishCategory>
{
    private readonly DishCategory _category = new();

    public DishCategoryBuilder WithRestaurantId(Guid id) { _category.RestaurantId = id; return this; }
    public DishCategoryBuilder WithCategoryName(string name) { _category.CategoryName = name; return this; }
    public DishCategoryBuilder WithDishes(ICollection<Dish> dishes) { _category.Dishes = dishes; return this; }
    public DishCategory Build() => _category;
}
