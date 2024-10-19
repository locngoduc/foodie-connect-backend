namespace foodie_connect_backend.Sessions.Dtos;

public record LoginResponse()
{
    public string Message { get; init; } = string.Empty;
};