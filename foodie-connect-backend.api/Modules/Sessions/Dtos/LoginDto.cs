using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Modules.Sessions.Dtos;

public record LoginDto()
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LoginType LoginType { get; init; }

    [Required] public string UserName { get; init; } = null!;

    [Required] public string Password { get; init; } = null!;
};