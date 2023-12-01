using System.Text.Json.Serialization;

namespace HolyShift.Dto;

public class SingUpRequestDto
{
    [JsonPropertyName("email")]
    public string Email { get; init; }

    [JsonPropertyName("password")]
    public string Password { get; init; }

    [JsonPropertyName("username")]
    public string UserName { get; init; }
}
