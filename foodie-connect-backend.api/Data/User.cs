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

    [MaxLength(128)]
    public string? AvatarId { get; set; } = String.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(16)]
    public string Role { get; set; } = "User";
}
