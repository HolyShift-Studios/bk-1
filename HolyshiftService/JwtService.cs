using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly IOptions<AuthConfig> _authConfig;
    private readonly byte[] _keyBytes = new byte[32];

    public JwtService(IOptions<AuthConfig> authConfig)
    {
        _authConfig = authConfig;
        _keyBytes = Convert.FromBase64String(_authConfig.Value.Key);
    }

    public string GenerateJwtToken(UserDbModel user)
    {
        var config = _authConfig.Value;
        var securityKey = new SymmetricSecurityKey(_keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(_keyBytes);

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
