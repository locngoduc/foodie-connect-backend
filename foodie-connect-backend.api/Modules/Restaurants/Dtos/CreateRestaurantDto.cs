using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Enums;
using System.ComponentModel;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace foodie_connect_backend.Modules.Restaurants.Dtos;

/// <summary>
/// Data transfer object for creating or updating a restaurant
/// </summary>
public record CreateRestaurantDto
{
    /// <summary>
    /// The name of the restaurant
    /// </summary>
    /// <example>Delicious Restaurant</example>
    [Required]
    [MaxLength(64)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Restaurant opening time
    /// </summary>
    /// <example>08:00:00</example>
    [DefaultValue("08:00:00")]
    public TimeOnly OpenTime { get; set; } = new(8, 0);

    /// <summary>
    /// Restaurant closing time
    /// </summary>
    /// <example>22:00:00</example>
    [DefaultValue("22:00:00")]
    public TimeOnly CloseTime { get; set; } = new(22, 0);

    /// <summary>
    /// Comma-separated longitude and latitude coordinates
    /// </summary>
    /// <example>103.8198,1.3521</example>
    [Required]
    public string LongitudeLatitude { get; set; } = null!;
    
    /// <summary>
    /// Current operational status of the restaurant
    /// </summary>
    /// <example>Open</example>
    [DefaultValue(RestaurantStatus.Open)]
    public RestaurantStatus Status { get; set; } = RestaurantStatus.Open;

    /// <summary>
    /// Restaurant contact number
    /// </summary>
    /// <example>0123456789</example>
    [Required]
    [MinLength(10)]
    [MaxLength(10)]
    [Phone]
    public string Phone { get; set; } = null!;
}