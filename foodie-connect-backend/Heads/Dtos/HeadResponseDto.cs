namespace foodie_connect_backend.Heads.Dtos;

public record HeadResponseDto()
{
    public string Id { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? EmailConfirmed { get; set; }
}