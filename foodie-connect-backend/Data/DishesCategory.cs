using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public partial class DishesCategory
{
    public int Id { get; set; }

    public int? Dishid { get; set; }

    public int? Categoryid { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Dish? Dish { get; set; }
}
