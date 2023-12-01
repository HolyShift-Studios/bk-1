using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using HolyShift.Auth;
using HolyShift.Dto;


namespace HolyShift.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/signup", SignUp);
        app.MapPost("/api/auth/signin", SignIn);
    }

    private static async Task<ResponseDto> SignUp(SingUpRequestDto requestDto, IAuthService AuthService)
    {
        if (string.IsNullOrEmpty(requestDto.Email) || string.IsNullOrEmpty(requestDto.Password) || string.IsNullOrEmpty(requestDto.UserName))
        {
            return ResponseDto.Error<ResponseDto>("1001", "Invalid request body");
        }
        return await AuthService.SignUp(requestDto);
    }
    private static async Task<SignInResponseDto> SignIn(SignInRequestDto requestDto, IAuthService AuthService)
    {
        if (string.IsNullOrEmpty(requestDto.Email) || string.IsNullOrEmpty(requestDto.Password))
        {
            return ResponseDto.Error<SignInResponseDto>("1001", "Invalid request body");
        }

        return await AuthService.SignIn(requestDto);
    }

}
