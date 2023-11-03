using System.Text.Json.Serialization;

namespace HolyShiftService.Dto
{
    public struct SingUpRequestDto
    {
        [JsonPropertyName("Password")]
        public string Email { get; init; }

        [JsonPropertyName("Password")]
        public string Password { get; init; }

        [JsonPropertyName("Username")]
        public string UserName { get; init; } 
    }
}