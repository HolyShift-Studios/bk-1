using HolyShift.Database.Models;

namespace HolyShift.Database;

public interface IUserDao
{
    Task<UserDbModel?> GetUserByEmailOrUserName(string email, string userName);
    Task AddUser(UserDbModel user);
}
