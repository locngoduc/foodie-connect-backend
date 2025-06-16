using foodie_connect_backend.Data;

namespace foodie_connect_backend.Modules.Dishes.Strategies;

public interface IDishSortStrategy
{
    IOrderedEnumerable<Dish> Sort(IEnumerable<Dish> dishes);
} 