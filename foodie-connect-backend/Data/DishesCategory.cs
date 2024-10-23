using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public class DishesCategory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string? DishId { get; set; }

    public string? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Dish? Dish { get; set; }
}
