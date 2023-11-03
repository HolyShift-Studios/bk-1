using System.Text.Json.Serialization;

namespace HolyShiftService.Dto
{
    public struct SignInRequestDto
    {
        [JsonPropertyName("Email")]
        public string Email { get; init; }

        [JsonPropertyName("Password")]
        public string Password { get; init; }
    }
}