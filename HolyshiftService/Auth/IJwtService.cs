using HolyShift.Database.Models;

namespace HolyShift.Auth;
public interface IJwtService
{
    string GenerateJwtToken(UserDbModel user);
}
