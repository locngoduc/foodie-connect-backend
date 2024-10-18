using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Data;

public class Head : IdentityUser
{
    [Required] 
    [MaxLength(50)]
    public string DisplayName { get; set; } = null!;

    [Required] 
    [MinLength(10)] [MaxLength(10)] 
    public string Phone { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}