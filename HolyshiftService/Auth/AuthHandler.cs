using HolyShift.Database;
using HolyShift.Database.Models;
using HolyShift.Dto;

namespace HolyShift.Auth;

public class AuthService : IAuthService
{
    private readonly IUserDao _userDao;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtService _jwtService;

    public AuthService(IUserDao userDao, IPasswordHashService passwordHashService, IJwtService jwtService)
    {
        _userDao = userDao;
        _passwordHashService = passwordHashService;
        _jwtService = jwtService;
    }

    public async Task<ResponseDto> SignUp(SingUpRequestDto signupRequest)
    {
        var existingUser = await _userDao.GetUserByEmailOrUserName(signupRequest.Email, signupRequest.UserName);

        if (existingUser != null)
            return ResponseDto.Error<ResponseDto>("1001", "User already exists with the same email or username");

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
        return new ResponseDto();
    }

    public async Task<SignInResponseDto> SignIn(SignInRequestDto requestDto)
    {
        var user = await _userDao.GetUserByEmailOrUserName(requestDto.Email, null);
        if (user == null)
        {
            return ResponseDto.Error<SignInResponseDto>("1001", "User not found");
        }
            
        var hashedPassword = await _passwordHashService.HashPassword(requestDto.Password, user.Salt);
        if(user.PasswordHash != hashedPassword)
        {
            return ResponseDto.Error<SignInResponseDto>("1002", "Invalid password");
        }
        else
        {
            var accessToken = _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);
            return new SignInResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }
    }
}