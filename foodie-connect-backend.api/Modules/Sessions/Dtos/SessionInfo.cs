namespace foodie_connect_backend.Modules.Sessions.Dtos;

public record SessionInfo()
{
    public string Type { get; init; } = string.Empty;
    public string Id { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Avatar { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool EmailConfirmed { get; init; }
};