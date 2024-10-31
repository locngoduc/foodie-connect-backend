using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Shared.Dtos;

public record ChangePasswordDto()
{
    [Required] public string OldPassword { get; init; } = null!;

    [Required] public string NewPassword { get; init; } = null!;
}