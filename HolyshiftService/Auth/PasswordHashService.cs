using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

public class PasswordHashService : IPasswordHashService
{
    public string GenerateRandomSalt(int size)
    {
        byte[] salt = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(salt);

        return Convert.ToBase64String(salt);
    }

    public async Task<string> HashPassword(string password, string saltBase64)
    {
        byte[] salt = Convert.FromBase64String(saltBase64);
        // Recommended settings by OWASP => https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#argon2id
        using var hasher = new Argon2id(Encoding.UTF8.GetBytes(password));
        hasher.Salt = salt;
        hasher.DegreeOfParallelism = 1;
        hasher.MemorySize = 9 * 1024; // 9 MiB
        hasher.Iterations = 4;
        var hashedPassword = await hasher.GetBytesAsync(64);

        var hashedPasswordBase64 = Convert.ToBase64String(hashedPassword);
        return hashedPasswordBase64;
    }
}