using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Restaurants.Dtos;

public record RestaurantResponseDto()
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int OpenTime { get; set; }
    public int CloseTime { get; set; }
    public RestaurantStatus Status { get; set; }
    public ICollection<SocialLink> SocialLinks { get; set; }
    public string Phone { get; set; }
    public IList<string> Images { get; set; }
    public string? FormattedAddress { get; set; }  
    public double Latitude { get; set; }    
    public double Longitude { get; set; }   
    public string HeadId { get; set; }
    public DateTime CreatedAt { get; set; }
}