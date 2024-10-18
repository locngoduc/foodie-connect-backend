using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Data;

public class Restaurant
{
    public int Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string Name { get; set; } = null!;

    public DateTime OpenTime { get; set; }

    public DateTime CloseTime { get; set; }

    [Required]
    [MaxLength(256)]
    public string Address { get; set; } = null!;

    public RestaurantStatus Status { get; set; } = RestaurantStatus.Open;

    public ICollection<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();

    [Required]
    [MinLength(10)] [MaxLength(10)]
    public string Phone { get; set; } = null!;
    
    [MaxLength(16)]
    public ICollection<string> Images { get; set; } = new List<string>();

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int? AreaId { get; set; }

    public virtual Area? Area { get; set; }

    public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
