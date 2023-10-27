public interface IUserDao
{
    Task<UserDbModel> GetUserByEmailOrUserName(string email, string userName);
    Task AddUser(UserDbModel user);
}
