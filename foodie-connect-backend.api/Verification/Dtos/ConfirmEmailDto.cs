using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Verification.Dtos;

public record ConfirmEmailDto()
{
    [Required]
    public string EmailToken { get; init; } = null!;
};