using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Heads.Dtos;

public record CreateHeadDto()
{
    [Required]
    [MaxLength(50)]
    public string DisplayName { get; init; } = null!;
    
    [Required]
    [MaxLength(16)]
    public string UserName { get; init; } = null!;
    
    [Required]
    [MaxLength(10)]
    [MinLength(10)]
    public string PhoneNumber { get; init; } = null!;
    
    [Required]
    [EmailAddress]
    public string Email { get; init; } = null!;
    
    [Required]
    [MaxLength(64)]
    public string Password { get; init; } = null!;
};