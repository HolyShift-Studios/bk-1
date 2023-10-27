using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;


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

public class LoginController : Controller
{
    private readonly HolyShiftDbContext _db;
    private readonly AuthService _jwtService;
    private readonly PasswordHashService _passwordHashService;
    private readonly IUserDao _userDao;

    public LoginController(HolyShiftDbContext context, AuthService jwtService, PasswordHashService passwordHashService, IUserDao userDao)
    {
        _db = context;
        _jwtService = jwtService;
        _passwordHashService = passwordHashService;
        _userDao = userDao;
    }

    [HttpPost("api/SignIn")]
    public async Task<ResponseDto> SignIn()
    {
        var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
        var request = JsonConvert.DeserializeObject<RequestDto>(requestBody);
        var responseDto = new ResponseDto();

        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            responseDto.Message = "Invalid request body";
            return responseDto;
        }
        var user = await _userDao.GetUserByEmailOrUserName(request.Email, null);
        if (user != null)
        {
            var salt = Convert.FromBase64String(user.Salt);
            var (hashedPassword, _) = _passwordHashService.HashPassword(request.Password, user.Salt);

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
}
