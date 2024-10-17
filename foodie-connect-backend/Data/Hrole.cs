using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public partial class Hrole
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<HeadsHrole> HeadsHroles { get; set; } = new List<HeadsHrole>();
}
