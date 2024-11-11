using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Modules.Promotions.Dtos;

public record CreatePromotionDto
{
    [MaxLength(128)] public string Name { get; set; } = null!;
    [MaxLength(2048)] public string Description { get; set; } = null!;
    public ICollection<string> Targets { get; set; } = new List<string>();
    public ICollection<CreatePromotionDetailsDto> PromotionDetails { get; set; } = new List<CreatePromotionDetailsDto>();
    public DateTime BeginsAt { get; set; }
    public DateTime EndsAt { get; set; }
}