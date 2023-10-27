using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

public class RegistrationController : Controller
{
    private readonly HolyShiftDbContext _db;
    private readonly AuthService _jwtService;
    private readonly PasswordHashService _passwordHashService;
    private readonly IUserDao _userDao;

    public RegistrationController(HolyShiftDbContext context, AuthService jwtService, PasswordHashService passwordHashService, IUserDao userDao)
    {
        _db = context;
        _jwtService = jwtService;
        _passwordHashService = passwordHashService;
        _userDao = userDao;
    }

    [HttpPost("api/SignUp")]
    public async Task<IActionResult> SignUp()
    {
        var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
        var requestData = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);
        if (requestData == null || !requestData.TryGetValue("email", out var email) || !requestData.TryGetValue("password", out var password) ||!requestData.TryGetValue("userName", out var userName))
        {
            return BadRequest(new { Message = "Invalid request body" });
        }
        
        var existingUser = await _userDao.GetUserByEmailOrUserName(email, userName);

        if (existingUser != null)
        {
            if (existingUser.Email == email)
                return BadRequest(new { Message = "Email is already registered." });
            if (existingUser.UserName == userName)
                return BadRequest(new { Message = "Username is already registered." });
        }
        var (hashedPassword, salt) = _passwordHashService.HashPassword(password);

        var user = new UserDbModel
        {
            Id = Guid.NewGuid(), 
            UserName = userName, 
            Email = email,
            Salt = salt,
            PasswordHash = hashedPassword,
            Role = UserRole.Customer
        };

        await _userDao.AddUser(user);
        return Ok(new { Message = "Registration successful" });
    }
}
