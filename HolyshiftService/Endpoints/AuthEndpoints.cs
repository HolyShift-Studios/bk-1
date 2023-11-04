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

    private static async Task<ResponseDto> SignUp(SingUpRequestDto requestDto, IAuthHandler authHandler)
    {
        if (string.IsNullOrEmpty(requestDto.Email) || string.IsNullOrEmpty(requestDto.Password) || string.IsNullOrEmpty(requestDto.UserName))
        {
            return ResponseDto.Error("1001", "Invalid request body");
        }
        return await authHandler.SignUp(requestDto);
    }
    private static async Task<SignInResponseDto> SignIn(SignInRequestDto requestDto, IAuthHandler authHandler)
    {
        if (string.IsNullOrEmpty(requestDto.Email) || string.IsNullOrEmpty(requestDto.Password))
        {
            return new SignInResponseDto
            {
                ErrorCode = "1001",
                Message = "Invalid request body"
            };
        }

        return await authHandler.SignIn(requestDto);
    }

}
