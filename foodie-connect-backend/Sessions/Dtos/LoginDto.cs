using Microsoft.Build.Framework;

namespace foodie_connect_backend.Sessions.Dtos;

public record LoginDto()
{
    [Required]
    public string UserName { get; init; }

    [Required]
    public string Password { get; init; }
};