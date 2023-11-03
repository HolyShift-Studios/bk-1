using HolyShift.Database;
using HolyShift.Database.Models;
using HolyShift.Dto;

namespace HolyShift.Auth;

public class AuthHandler : IAuthHandler
{
    private readonly IUserDao _userDao;
    private readonly PasswordHashService _passwordHashService;

    public AuthHandler(IUserDao userDao, PasswordHashService passwordHashService, JwtService jwtService)
    {
        _userDao = userDao;
        _passwordHashService = passwordHashService;
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
        return new ResponseDto();
    }
}

public interface IAuthHandler
{
    Task<ResponseDto> SignUp(SingUpRequestDto signupRequest);
}
