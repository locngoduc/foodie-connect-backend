using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public partial class Service
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Restaurantid { get; set; }

    public virtual Restaurant? Restaurant { get; set; }
}
