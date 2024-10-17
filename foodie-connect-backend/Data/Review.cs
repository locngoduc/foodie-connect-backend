using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public partial class Review
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public int Rating { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Dishid { get; set; }

    public virtual Dish? Dish { get; set; }
}
