using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Modules.Promotions.Dtos;

public record CreatePromotionDto
{
    [Required] public string Name { get; set; }

    [Required] public string Target { get; set; }

    [Required] public DateTime ExpiredAt { get; set; }

    [Required] public Guid DishId { get; set; }

    [Required] public Guid RestaurantId { get; set; }
}