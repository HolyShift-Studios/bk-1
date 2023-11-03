using System.Text.Json.Serialization;

namespace HolyShift.Dto;

public class SignInResponseDto : ResponseDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; }
}
