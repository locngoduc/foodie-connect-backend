using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Dtos;

namespace foodie_connect_backend.Modules.Dishes.Strategies;

public class RatingSortStrategy : IDishSortStrategy
{
    private readonly bool _ascending;
    private readonly Dictionary<Guid, ScoreResponseDto> _scores;

    public RatingSortStrategy(Dictionary<Guid, ScoreResponseDto> scores, bool ascending = false)
    {
        _ascending = ascending;
        _scores = scores;
    }

    public IOrderedEnumerable<Dish> Sort(IEnumerable<Dish> dishes)
    {
        return _ascending
            ? dishes.OrderBy(d => _scores.GetValueOrDefault(d.Id, new ScoreResponseDto()).AverageRating)
            : dishes.OrderByDescending(d => _scores.GetValueOrDefault(d.Id, new ScoreResponseDto()).AverageRating);
    }
} 