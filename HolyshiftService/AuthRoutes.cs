using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using HolyShiftService.Dto;

public static class AuthenticationRoutes
{
    public static async Task<SignInResponseDto> SignIn(HttpContext context, AuthManager authManager)
    {
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var requestDto = JsonConvert.DeserializeObject<SignInRequestDto>(requestBody);
        SignInResponseDto response = await authManager.SignIn(requestDto);
        return response;
    }

    public static async Task<SingUpResponseDto> SignUp(HttpContext context, AuthManager authManager)
    {
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var requestDto = JsonConvert.DeserializeObject<SingUpRequestDto>(requestBody);
        SingUpResponseDto response = await authManager.SignUp(requestDto);
        return response;
    }
}
