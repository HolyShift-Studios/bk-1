using System.Text.Json.Serialization;

namespace HolyShift.Dto;

public class SignInRequestDto
{
    [JsonPropertyName("email")]
    public string Email { get; init; }

    [JsonPropertyName("password")]
    public string Password { get; init; }
}
