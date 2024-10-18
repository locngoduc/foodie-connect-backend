using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Heads.Dtos;

public record CreateHeadDto()
{
    [Required]
    [MaxLength(50)]
    public string DisplayName { get; set; } = null!;
    
    [Required]
    [MaxLength(16)]
    public string UserName { get; set; } = null!;
    
    [Required]
    [MaxLength(10)]
    [MinLength(10)]
    public string PhoneNumber { get; set; } = null!;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    [MaxLength(64)]
    public string Password { get; set; } = null!;
};