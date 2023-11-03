using System;
using System.Threading.Tasks;
using HolyShift.Auth;
using HolyShift.Database;
using HolyShift.Database.Models;
using HolyShift.Dto;
using Newtonsoft.Json;

public class AuthManager
{
    private readonly HolyShiftDbContext _db;
    private readonly JwtService _jwtService;
    private readonly PasswordHashService _passwordHashService;
    private readonly IUserDao _userDao;

    public AuthManager(HolyShiftDbContext db, JwtService jwtService, PasswordHashService passwordHashService, IUserDao userDao)
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

        var hashedPassword = await _passwordHashService.HashPassword(requestDto.Password, user.Salt);
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
}
