using System.Threading.Tasks;
using HolyShift.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HolyShift.Database
{
    public class UserDao : IUserDao
    {
        private readonly HolyShiftDbContext _db;
        
        public UserDao(HolyShiftDbContext context)
        {
            _db = context;
        }

        public async Task<UserDbModel?> GetUserByEmailOrUserName(string email, string userName)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email || u.UserName == userName);
        }

        public async Task AddUser(UserDbModel user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
    }
}
