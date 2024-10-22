using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Restaurants.Dtos;

public record CreateRestaurantDto
{
    [Required] [MaxLength(64)] public string Name { get; set; } = null!;

    public int OpenTime { get; set; } = 8;

    public int CloseTime { get; set; } = 22;

    [Required] [MaxLength(256)] public string Address { get; set; } = null!;

    public RestaurantStatus Status { get; set; } = RestaurantStatus.Open;

    [Required]
    [MinLength(10)]
    [MaxLength(10)]
    [Phone]
    public string Phone { get; set; } = null!;

    [Required] public IFormFile Logo { get; set; } = null!;
    [Required] public IFormFile Banner { get; set; } = null!;


    public ICollection<IFormFile>? Images { get; set; }

    //TODO: Picking coordination in map
    //public int? AreaId { get; set; }
}