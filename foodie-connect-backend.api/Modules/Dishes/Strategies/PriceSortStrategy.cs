using foodie_connect_backend.Data;

namespace foodie_connect_backend.Modules.Dishes.Strategies;

public class PriceSortStrategy : IDishSortStrategy
{
    private readonly bool _ascending;

    public PriceSortStrategy(bool ascending = true)
    {
        _ascending = ascending;
    }

    public IOrderedEnumerable<Dish> Sort(IEnumerable<Dish> dishes)
    {
        return _ascending 
            ? dishes.OrderBy(d => d.Price)
            : dishes.OrderByDescending(d => d.Price);
    }
} 