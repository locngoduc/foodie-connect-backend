using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Promotions.Dtos;

public record CreatePromotionDto
{
    [Required] public string Name { get; set; }

    [Required] public string Target { get; set; }

    [Required] public DateTime ExpiredAt { get; set; }

    [Required] public string DishId { get; set; }

    [Required] public string RestaurantId { get; set; }
}