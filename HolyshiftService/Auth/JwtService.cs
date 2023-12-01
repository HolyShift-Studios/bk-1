using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using HolyShift.Config;
using HolyShift.Database;
using HolyShift.Database.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HolyShift.Auth;

public class JwtService : IJwtService
{
    private readonly IOptions<AuthConfig> _authConfig;
    private readonly IUserDao _userDao;

    public JwtService(IOptions<AuthConfig> authConfig, IUserDao userDao)
    {
        _authConfig = authConfig;
        _userDao = userDao;
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
}
