namespace foodie_connect_backend.Sessions.Dtos;

public record GenericResponse()
{
    public string Message { get; init; } = string.Empty;
};