using HolyShift.Database;
using HolyShift.Database.Models;
using HolyShift.Dto;

namespace HolyShift.Auth;

public class AuthHandler : IAuthHandler
{
    private readonly IUserDao _userDao;
    private readonly PasswordHashService _passwordHashService;
    private readonly JwtService _jwtService;

    public AuthHandler(IUserDao userDao, PasswordHashService passwordHashService, JwtService jwtService)
    {
        _userDao = userDao;
        _passwordHashService = passwordHashService;
        _jwtService = jwtService;
    }

    public async Task<ResponseDto> SignUp(SingUpRequestDto signupRequest)
    {
        var existingUser = await _userDao.GetUserByEmailOrUserName(signupRequest.Email, signupRequest.UserName);

        if (existingUser != null)
            return ResponseDto.Error("1001", "User already exist with the same email or username");

        string salt = _passwordHashService.GenerateRandomSalt(16);
        string hashedPassword = await _passwordHashService.HashPassword(signupRequest.Password, salt);

        var user = new UserDbModel
        {
            Id = Guid.NewGuid(),
            UserName = signupRequest.UserName,
            Email = signupRequest.Email,
            Salt = salt,
            PasswordHash = hashedPassword,
            Role = UserRole.Customer
        };

        await _userDao.AddUser(user);
        return new ResponseDto{Message = "success" };
    }

    public async Task<SignInResponseDto> SignIn(SignInRequestDto requestDto)
    {
        var user = await _userDao.GetUserByEmailOrUserName(requestDto.Email, null);
        if (user == null)
        {
            return new SignInResponseDto { 
                ErrorCode = "1001",
                Message = "User not found"
            };
        }
            
        var hashedPassword = await _passwordHashService.HashPassword(requestDto.Password, user.Salt);
        if (user.PasswordHash == hashedPassword)
        {
            var accessToken = _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);
            return new SignInResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Message = "Success"
            };
        }
        else
        {
            return new SignInResponseDto
            {
                ErrorCode = "1002",
                Message = "Invalid password"
            };
        }
    }
}

public interface IAuthHandler
{
    Task<ResponseDto> SignUp(SingUpRequestDto signupRequest);
    Task<SignInResponseDto> SignIn(SignInRequestDto signinRequest);
}
