namespace foodie_connect_backend.Modules.Users.Dtos;

public record UserResponseDto()
{
    public string Id { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Avatar { get; set; }
}