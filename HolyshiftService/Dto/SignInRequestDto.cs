using System.Text.Json.Serialization;

namespace HolyShift.Dto;

public class SignInRequestDto
{
    [JsonPropertyName("Email")]
    public string Email { get; init; }

    [JsonPropertyName("Password")]
    public string Password { get; init; }
}
