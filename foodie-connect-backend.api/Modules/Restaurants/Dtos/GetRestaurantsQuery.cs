namespace foodie_connect_backend.Modules.Restaurants.Dtos;

/// <summary>
/// Query parameters for getting restaurants
/// </summary>
public class GetRestaurantsQuery
{
    /// <summary>
    /// Geographic coordinates in format "longitude,latitude" (e.g. "144.9631,-37.8136")
    /// </summary>
    public string? Origin { get; init; }

    /// <summary>
    /// Search radius in meters. Defaults to 500 if not specified.
    /// </summary>
    public int? Radius { get; init; } = 500;

    /// <summary>
    /// Filter restaurants by owner ID
    /// </summary>
    public string? OwnerId { get; init; }
    
    /// <summary>
    /// Filter restaurants by name
    /// </summary>
    public string? Name { get; init; }
}