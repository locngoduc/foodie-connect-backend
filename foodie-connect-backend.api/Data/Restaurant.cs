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

    [Required] public int OpenTime { get; set; }

    [Required] public int CloseTime { get; set; }

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
    [Required] [JsonIgnore] public Point Location { get; init; } = null!;

    [JsonIgnore] public Area? Area { get; init; }

    [JsonIgnore] public ICollection<Dish> Dishes { get; init; } = new List<Dish>();

    [JsonIgnore] public ICollection<DishReview> Reviews { get; init; } = new List<DishReview>();

    [JsonIgnore] public ICollection<Promotion> Promotions { get; init; } = new List<Promotion>();

    [JsonIgnore] public ICollection<Service> Services { get; init; } = new List<Service>();

    [MaxLength(128)] public string HeadId { get; init; } = null!;
}