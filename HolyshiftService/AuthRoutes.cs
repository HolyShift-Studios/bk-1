using HolyShift.Dto;
using Newtonsoft.Json;

namespace HolyShift;

public static class AuthenticationRoutes
{
    public static async Task<SignInResponseDto> SignIn(HttpContext context, AuthManager authManager)
    {
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var requestDto = JsonConvert.DeserializeObject<SignInRequestDto>(requestBody);
        SignInResponseDto response = await authManager.SignIn(requestDto);
        return response;
    }
}
