using System.Text.Json.Serialization;
using foodie_connect_backend.Shared.Enums;
using Microsoft.Build.Framework;

namespace foodie_connect_backend.Sessions.Dtos;

public record LoginDto()
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LoginType LoginType { get; init; }
    
    [Required]
    public string UserName { get; init; }

    [Required]
    public string Password { get; init; }
};