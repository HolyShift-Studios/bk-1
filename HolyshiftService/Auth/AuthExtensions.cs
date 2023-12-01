using HolyShift.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HolyShift.Auth;

public static class AuthExtensions
{
    public static void AddAuth(this WebApplicationBuilder builder)
    {
        var config = new AuthConfig();
        builder.Configuration.GetSection("Auth").Bind(config);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config.Issuer,
                    ValidAudience = config.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(config.KeyBase64)),
                };
            });
    }
}
