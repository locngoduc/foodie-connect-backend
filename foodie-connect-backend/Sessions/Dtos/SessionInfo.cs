namespace foodie_connect_backend.Sessions.Dtos;

public record SessionInfo()
{
    public string Type { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
};