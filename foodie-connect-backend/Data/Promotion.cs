using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public partial class Promotion
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Target { get; set; }

    public DateTime? Expireddate { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Restaurantid { get; set; }

    public int? Dishid { get; set; }

    public virtual Dish? Dish { get; set; }

    public virtual Restaurant? Restaurant { get; set; }
}
