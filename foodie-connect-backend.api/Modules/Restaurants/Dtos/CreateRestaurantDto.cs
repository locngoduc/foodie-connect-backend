using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Modules.Restaurants.Dtos;

public record CreateRestaurantDto
{
    [Required][MaxLength(64)] public string Name { get; set; } = null!;

    public int OpenTime { get; set; } = 8;

    public int CloseTime { get; set; } = 22;

    public string LatitudeLongitude { get; set; } = null!;
    
    public RestaurantStatus Status { get; set; } = RestaurantStatus.Open;

    [Required]
    [MinLength(10)]
    [MaxLength(10)]
    [Phone]
    public string Phone { get; set; } = null!;
}