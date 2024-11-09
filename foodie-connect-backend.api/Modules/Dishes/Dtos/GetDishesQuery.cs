

using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Modules.Dishes.Dtos;

public class GetDishesQuery
{
    [Required(ErrorMessage = "Restaurant ID is required")]
    public Guid RestaurantId { get; set; }
    
    public string? Categories { get; set; }

    public decimal? MinPrice { get; set; } = null;

    public decimal? MaxPrice { get; set; } = null;
}