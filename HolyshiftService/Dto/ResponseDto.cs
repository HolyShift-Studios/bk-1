using System.Text.Json.Serialization;

namespace HolyShift.Dto;

public class ResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;

    [JsonPropertyName("error_code")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    public static ResponseDto Error(string errorCode, string message)
    {
        return new ResponseDto
        {
            Success = false,
            ErrorCode = errorCode,
            Message = message,
        };
    }
}
