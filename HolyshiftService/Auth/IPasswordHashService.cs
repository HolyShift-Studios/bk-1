
public interface IPasswordHashService
{
    string GenerateRandomSalt(int size);
    Task<string> HashPassword(string password, string saltBase64);
}
