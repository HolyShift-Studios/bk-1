using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;



public class AuthService
{
    private readonly IOptions<AuthConfig> _authConfig;
    private readonly HolyShiftDbContext _db;

    public AuthService(IOptions<AuthConfig> authConfig ,HolyShiftDbContext db)
    {
        _authConfig = authConfig;
        _db = db;
    }

    public TokenValidationParameters GetTokenValidationParameters()
    {
        var config = _authConfig.Value;
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(config.KeyBase64));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config.Issuer,
            ValidAudience = config.Audience,
            IssuerSigningKey = securityKey,
        };
        return tokenValidationParameters;
    }
    public string GenerateJwtToken(UserDbModel user)
    {
        var config = _authConfig.Value;
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(config.KeyBase64));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            config.Issuer,
            config.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(config.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(UserDbModel user)
    {
        var tokenLength = 16; 
        var randomBytes = new byte[tokenLength];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        string refreshToken = BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
        user.RefreshToken = refreshToken;

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(_authConfig.Value.RefreshTokenExpirationDays); 
        _db.Entry(user).State = EntityState.Modified;
        _db.SaveChanges();
        return refreshToken;
    }


    public bool ValidateRefreshToken(string refreshToken)
    {
        var user = _db.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiration < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Convert.FromBase64String(_authConfig.Value.KeyBase64));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _authConfig.Value.Issuer,
            ValidAudience = _authConfig.Value.Audience,
            IssuerSigningKey = key,
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
