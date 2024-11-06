using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Modules.Users.Dtos;

public record CreateUserDto()
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
    
    /// <summary>
    /// Password should contain at least one uppercase letter, one number and one special character
    /// </summary>
    [Required]
    [MaxLength(64)]
    public string Password { get; init; } = null!;
};