namespace foodie_connect_backend.Modules.Heads.Dtos;

public record HeadResponseDto()
{
    public string Id { get; init; }
    public string? UserName { get; init; }
    public string? DisplayName { get; init; }
    public string? Avatar { get; init; }
}