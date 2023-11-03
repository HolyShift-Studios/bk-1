using HolyShift.Auth;
using HolyShift.Dto;

namespace HolyShift.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/signup", SignUp);
    }

    private static async Task<ResponseDto> SignUp(SingUpRequestDto requestDto, IAuthHandler authHandler)
    {
        // TODO: CheckAutomated Validation
        return await authHandler.SignUp(requestDto);
    }
}
