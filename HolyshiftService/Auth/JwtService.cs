using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using HolyShift.Config;
using HolyShift.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HolyShift.Auth;

public class JwtService
{
    private readonly IOptions<AuthConfig> _authConfig;
    private readonly HolyShiftDbContext _db;

    public JwtService(IOptions<AuthConfig> authConfig, HolyShiftDbContext db)
    {
        _authConfig = authConfig;
        _db = db;
    }

    public string GenerateJwtToken(UserDbModel user)
    {
        var config = _authConfig.Value;
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(config.KeyBase64));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
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
}
