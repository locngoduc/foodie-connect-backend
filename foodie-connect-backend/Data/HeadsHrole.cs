using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public partial class HeadsHrole
{
    public int Id { get; set; }

    public int? Hroleid { get; set; }

    public int? Headid { get; set; }

    public virtual Head? Head { get; set; }

    public virtual Hrole? Hrole { get; set; }
}
