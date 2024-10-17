using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public partial class Restaurant
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime Opentime { get; set; }

    public DateTime Closetime { get; set; }

    public string? Address { get; set; }

    public string? Status { get; set; }

    public string? Link { get; set; }

    public string Phone { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Areaid { get; set; }

    public virtual Area? Area { get; set; }

    public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
