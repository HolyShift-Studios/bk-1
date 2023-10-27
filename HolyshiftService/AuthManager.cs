using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;


public class RequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class ResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Message { get; set; }
}
public class RegistereRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; } 
}

public class RegisterResponseDto
{
    public string Message { get; set; }
}

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

    public async Task<ResponseDto> SignIn(RequestDto requestDto)
    {
        var responseDto = new ResponseDto();

        if (string.IsNullOrEmpty(requestDto.Email) || string.IsNullOrEmpty(requestDto.Password))
        {
            responseDto.Message = "Invalid request body";
            return responseDto;
        }
        var user = await _userDao.GetUserByEmailOrUserName(requestDto.Email, null);
        if (user != null)
        {
            var salt = Convert.FromBase64String(user.Salt);
            var (hashedPassword, _) = _passwordHashService.HashPassword(requestDto.Password, user.Salt);

            if (user.PasswordHash == hashedPassword)
            {
                var accessToken = _jwtService.GenerateJwtToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken(user);
                responseDto.AccessToken = accessToken;
                responseDto.RefreshToken = refreshToken;
                responseDto.Message = "Success";
            }
            else
            {
                responseDto.Message = "Invalid password";
            }
        }
        else
        {
            responseDto.Message = "User not found";
        }

        return responseDto;
    }

    public async Task<RegisterResponseDto> SignUp(RegistereRequestDto requestDto)
    {
        var responseDto = new RegisterResponseDto();

        if (string.IsNullOrEmpty(requestDto.Email) || string.IsNullOrEmpty(requestDto.Password) || string.IsNullOrEmpty(requestDto.UserName))
        {
            responseDto.Message = "Invalid request body";
            return responseDto;
        }

        var existingUser = await _userDao.GetUserByEmailOrUserName(requestDto.Email, requestDto.UserName);

        if (existingUser != null)
        {
            if (existingUser.Email == requestDto.Email)
            {
                responseDto.Message = "Email is already registered.";
                return responseDto;
            }
            if (existingUser.UserName == requestDto.UserName)
            {
                responseDto.Message = "Username is already registered.";
                return responseDto;
            }
        }

        var (hashedPassword, salt) = _passwordHashService.HashPassword(requestDto.Password);

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
        responseDto.Message = "Registration successful";
        return responseDto;
    }
}