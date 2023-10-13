using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

public class RegistrationController : Controller
{
    private readonly HolyShiftDbContext _context;
    private readonly JwtService _jwtService;

    public RegistrationController(HolyShiftDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("api/register")]
    public async Task<IActionResult> Register()
    {
        var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
        var requestData = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);
        if (requestData == null || 
            !requestData.TryGetValue("email", out var email) || 
            !requestData.TryGetValue("password", out var password) ||
            !requestData.TryGetValue("userName", out var userName))
        {
            return BadRequest(new { Message = "Invalid request body" });
        }
        
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (existingUser != null)
        {
            return BadRequest(new { Message = "Email is already registered." });
        }

        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
        {
            hasher.Salt = salt;
            hasher.DegreeOfParallelism = 8;
            hasher.MemorySize = 65536;
            hasher.Iterations = 4;

            var hashedPassword = Convert.ToBase64String(hasher.GetBytes(32));

            var user = new UserDbModel
            {
                Id = Guid.NewGuid(), 
                UserName = userName, 
                Email = email,
                Salt = Convert.ToBase64String(salt),
                PasswordHash = hashedPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Registration successful." });
        }
    }
}
