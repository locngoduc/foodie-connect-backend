using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public class DishesCategory
{
    public int Id { get; set; }

    public int? DishId { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Dish? Dish { get; set; }
}
