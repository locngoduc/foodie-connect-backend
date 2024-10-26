namespace foodie_connect_backend.Heads.Dtos;

public record HeadResponseDto()
{
    public string Id { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Avatar { get; set; }
}