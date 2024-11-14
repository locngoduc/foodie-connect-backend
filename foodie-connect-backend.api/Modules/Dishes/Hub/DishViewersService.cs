namespace foodie_connect_backend.Modules.Dishes.Hub;

public class ActiveDishViewersService(ILogger<ActiveDishViewersService> logger)
{
    private readonly Dictionary<Guid, HashSet<string>> _activeDishViewers = new();
    private readonly Dictionary<string, HashSet<Guid>> _connectionToDishes = new();
    private readonly object _lock = new();
    private readonly ILogger<ActiveDishViewersService> _logger = logger;

    public int StartViewing(Guid dishId, string connectionId)
    {
        lock (_lock)
        {
            // Add to dish viewers
            if (!_activeDishViewers.ContainsKey(dishId))
            {
                _activeDishViewers[dishId] = new HashSet<string>();
            }
            _activeDishViewers[dishId].Add(connectionId);

            // Track which dishes this connection is viewing
            if (!_connectionToDishes.ContainsKey(connectionId))
            {
                _connectionToDishes[connectionId] = new HashSet<Guid>();
            }
            _connectionToDishes[connectionId].Add(dishId);

            return _activeDishViewers[dishId].Count;
        }
    }

    public Dictionary<Guid, int> RemoveViewerFromAllDishes(string connectionId)
    {
        var affectedDishes = new Dictionary<Guid, int>();
        
        lock (_lock)
        {
            if (_connectionToDishes.TryGetValue(connectionId, out var dishIds))
            {
                foreach (var dishId in dishIds)
                {
                    if (_activeDishViewers.TryGetValue(dishId, out var viewers))
                    {
                        viewers.Remove(connectionId);
                        affectedDishes[dishId] = viewers.Count;

                        if (viewers.Count == 0)
                        {
                            _activeDishViewers.Remove(dishId);
                        }
                    }
                }
                
                _connectionToDishes.Remove(connectionId);
            }
        }

        return affectedDishes;
    }

    public int GetCurrentViewers(Guid dishId)
    {
        lock (_lock)
        {
            return _activeDishViewers.TryGetValue(dishId, out var viewers) ? viewers.Count : 0;
        }
    }
}