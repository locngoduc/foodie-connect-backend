using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Enums;
using NetTopologySuite.Geometries;

namespace foodie_connect_backend.Data;

public sealed class Restaurant
{
    public Guid Id { get; set; }

    [Required] [MaxLength(128)] public string Name { get; set; } = null!;

    [Required] public TimeOnly OpenTime { get; set; }

    [Required] public TimeOnly CloseTime { get; set; }

    public RestaurantStatus Status { get; set; } = RestaurantStatus.Open;

    public ICollection<SocialLink> SocialLinks { get; init; } = new List<SocialLink>();

    [Required]
    [MinLength(10)]
    [MaxLength(10)]
    public string Phone { get; set; } = null!;

    [MaxLength(128)] public IList<string> Images { get; init; } = new List<string>();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    
    public Guid AreaId { get; set; }

    public bool IsDeleted { get; init; }

    public ICollection<DishCategory> DishCategories { get; init; } = new List<DishCategory>();

    [Column(TypeName="geography")]
    [Required] [JsonIgnore] public Point Location { get; set; } = null!;

    [JsonIgnore] public Area? Area { get; init; }

    public ICollection<Dish> Dishes { get; init; } = new List<Dish>();

    public ICollection<RestaurantReview> Reviews { get; init; } = new List<RestaurantReview>();

    [JsonIgnore] public ICollection<Promotion> Promotions { get; init; } = new List<Promotion>();

    [JsonIgnore] public ICollection<Service> Services { get; init; } = new List<Service>();

    [MaxLength(128)] public string HeadId { get; init; } = null!;
}