using HolyShift.Dto;
namespace HolyShift.Auth;
public interface IAuthService
{
    Task<ResponseDto> SignUp(SingUpRequestDto signupRequest);
    Task<SignInResponseDto> SignIn(SignInRequestDto signinRequest);
}