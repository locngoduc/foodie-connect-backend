using Microsoft.Build.Framework;

namespace foodie_connect_backend.Heads.Dtos;

public record ChangePasswordDto()
{
    [Required]
    public string OldPassword { get; init; }
    
    [Required]
    public string NewPassword { get; init; }
}