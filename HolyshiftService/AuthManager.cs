using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HolyShiftService.Dto;

public class AuthManager
{
    private readonly HolyShiftDbContext _db;
    private readonly AuthService _jwtService;
    private readonly PasswordHashService _passwordHashService;
    private readonly IUserDao _userDao;

    public AuthManager(HolyShiftDbContext db, AuthService jwtService, PasswordHashService passwordHashService, IUserDao userDao)
    {
        _db = db;
        _jwtService = jwtService;
        _passwordHashService = passwordHashService;
        _userDao = userDao;
    }

    public async Task<SignInResponseDto> SignIn(SignInRequestDto requestDto)
    {
        string accessToken = null;
        string refreshToken = null;
        string message = null;
        if (string.IsNullOrEmpty(requestDto.Email) || string.IsNullOrEmpty(requestDto.Password))
        {
            return new SignInResponseDto{
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Message = "Invalid request body"
                };
        }
        var user = await _userDao.GetUserByEmailOrUserName(requestDto.Email, null);
        if (user == null)
        {
            return new SignInResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Message = "User not found"
            };
        }

        var hashedPassword = _passwordHashService.HashPassword(requestDto.Password, user.Salt);
        if (user.PasswordHash == hashedPassword)
        {
            accessToken = _jwtService.GenerateJwtToken(user);
            refreshToken = _jwtService.GenerateRefreshToken(user);
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
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Message = "Invalid password"
            };
        }
    }

    public async Task<SingUpResponseDto> SignUp(SingUpRequestDto requestDto)
    {
        if (string.IsNullOrEmpty(requestDto.Email) || string.IsNullOrEmpty(requestDto.Password) || string.IsNullOrEmpty(requestDto.UserName))
        {
            return new SingUpResponseDto{Message = "Invalid request body"};
        }

        var existingUser = await _userDao.GetUserByEmailOrUserName(requestDto.Email, requestDto.UserName);

        if (existingUser != null)
        {
            if (existingUser.Email == requestDto.Email)
            {
                return new SingUpResponseDto{Message = "Email is already registered."};
            }
            if (existingUser.UserName == requestDto.UserName)
            {
                return new SingUpResponseDto{Message = "Username is already registered."};
            }
        }

        string salt = _passwordHashService.GenerateRandomSalt(16);
        string hashedPassword = _passwordHashService.HashPassword(requestDto.Password, salt);

        var user = new UserDbModel
        {
            Id = Guid.NewGuid(),
            UserName = requestDto.UserName,
            Email = requestDto.Email,
            Salt = salt,
            PasswordHash = hashedPassword,
            Role = UserRole.Customer
        };

        await _userDao.AddUser(user);
        return new SingUpResponseDto{Message = "Registration successful"};
    }
}