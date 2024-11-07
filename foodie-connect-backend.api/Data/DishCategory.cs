using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace foodie_connect_backend.Data;

[PrimaryKey(nameof(RestaurantId), nameof(CategoryName))]
public class DishCategory
{
    [MaxLength(128)] public string RestaurantId { get; set; } = null!;
    [Required] [MaxLength(32)] public string CategoryName { get; set; } = null!;
    
    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}