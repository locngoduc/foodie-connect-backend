using foodie_connect_backend.Data;

namespace foodie_connect_backend.Modules.Dishes.Strategies;

public class NameSortStrategy : IDishSortStrategy
{
    private readonly bool _ascending;

    public NameSortStrategy(bool ascending = true)
    {
        _ascending = ascending;
    }

    public IOrderedEnumerable<Dish> Sort(IEnumerable<Dish> dishes)
    {
        return _ascending
            ? dishes.OrderBy(d => d.Name)
            : dishes.OrderByDescending(d => d.Name);
    }
} 