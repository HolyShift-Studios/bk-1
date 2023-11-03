using System.Text.Json.Serialization;

namespace HolyShiftService.Dto
{
    public struct SignInResponseDto
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public string Message { get; init; }
    }
}