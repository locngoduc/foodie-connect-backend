using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Enums;
using foodie_connect_backend.SocialLinks.Dtos;

namespace foodie_connect_backend.Restaurants.Dtos;

public record RestaurantResponseDto
{
    [Required]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int OpenTime { get; set; }

    public int CloseTime { get; set; }

    public string Address { get; set; } = null!;

    public RestaurantStatus Status { get; set; }

    public IList<String> Images { get; set; } = new List<string>();

    public List<SocialLinkResponseDto> SocialLinks { get; set; } = new List<SocialLinkResponseDto>();

    public string HeadId { get; set; } = null!;

}