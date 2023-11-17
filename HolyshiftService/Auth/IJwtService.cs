using HolyShift.Database.Models;

namespace HolyShift.Auth;
public interface IJwtService
{
    string GenerateJwtToken(UserDbModel user);
    string GenerateRefreshToken(UserDbModel user);
    bool ValidateRefreshToken(string refreshToken);
}
