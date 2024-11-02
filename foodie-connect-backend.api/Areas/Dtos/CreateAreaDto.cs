using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Area.Dtos;

public record CreateAreaDto()
{
    [Required]
    public string RestaurantId { get; set; }
    
    [Required]
    public string LatitudeLongitude { get; set; }   
}