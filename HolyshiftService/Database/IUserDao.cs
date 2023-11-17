using HolyShift.Database.Models;
using System.Threading.Tasks;

namespace HolyShift.Database;
public interface IUserDao
{
    Task<UserDbModel?> GetUserByEmailOrUserName(string email, string userName);
    Task AddUser(UserDbModel user);
}
