using System;
using System.Collections.Generic;

namespace foodie_connect_backend.Data;

public partial class UsersUrole
{
    public int Id { get; set; }

    public int? Uroleid { get; set; }

    public int? Userid { get; set; }

    public virtual Urole? Urole { get; set; }

    public virtual User? User { get; set; }
}
