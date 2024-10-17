using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public partial class Dish
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Restaurantid { get; set; }

    public virtual ICollection<DishesCategory> DishesCategories { get; set; } = new List<DishesCategory>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual Restaurant? Restaurant { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
