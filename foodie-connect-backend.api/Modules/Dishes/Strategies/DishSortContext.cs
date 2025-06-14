using foodie_connect_backend.Data;

namespace foodie_connect_backend.Modules.Dishes.Strategies;

public class DishSortContext
{
    private readonly IDishSortStrategy _strategy;

    public DishSortContext(IDishSortStrategy strategy)
    {
        _strategy = strategy;
    }

    public IOrderedEnumerable<Dish> Sort(IEnumerable<Dish> dishes)
    {
        return _strategy.Sort(dishes);
    }
} 