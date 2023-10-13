using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

public class LoginController : Controller
{
    private readonly HolyShiftDbContext _context;
    private readonly JwtService _jwtService;
    private readonly ILogger<LoginController> _logger;

    public LoginController(HolyShiftDbContext context, JwtService jwtService, ILogger<LoginController> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("api/login")]
    public async Task<IActionResult> Login()
    {
        var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
        var requestData = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);

        if (requestData == null)
        {
            return BadRequest(new { Message = "Invalid request body" });
        }

        if (!requestData.TryGetValue("email", out var email) || !requestData.TryGetValue("password", out var password))
        {
            return BadRequest(new { Message = "Invalid request body" });
        }

        _logger.LogInformation("Login attempt for email: " + email);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null)
        {
            _logger.LogInformation("User found with email: " + email);
            var salt = Convert.FromBase64String(user.Salt);
            using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                hasher.Salt = salt;
                hasher.DegreeOfParallelism = 8;
                hasher.MemorySize = 65536;
                hasher.Iterations = 4;

                var hashedPassword = Convert.ToBase64String(hasher.GetBytes(32));

                if (user.PasswordHash == hashedPassword)
                {
                    _logger.LogInformation("Login successful for email: " + email);

                    var accessToken = _jwtService.GenerateJwtToken(user);
                    var refreshToken = _jwtService.GenerateRefreshToken();

                    return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
                }
                else
                {
                    _logger.LogInformation("Password does not match for email: " + email);
                    return BadRequest(new { Message = "Invalid password" });
                }
            }
        }
        else
        {
            _logger.LogInformation("User not found with email: " + email);
            return BadRequest(new { Message = "User not found" });
        }
    }
}
