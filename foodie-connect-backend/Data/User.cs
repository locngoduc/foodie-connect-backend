using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Data;

public class User : IdentityUser
{
    [Required] 
    [MaxLength(50)]
    public string DisplayName { get; set; } = null!;
    
    [MaxLength(256)]
    public string AvatarUrl { get; set; } = String.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
