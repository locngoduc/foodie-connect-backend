using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Data;

public partial class User :IdentityUser
{
    [Required]
    public string DisplayName { get; set; }

    [Required]
    public string Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
