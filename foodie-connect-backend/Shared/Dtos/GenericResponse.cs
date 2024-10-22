namespace foodie_connect_backend.Shared.Dtos;

public record GenericResponse()
{
    public string Message { get; init; } = string.Empty;
};