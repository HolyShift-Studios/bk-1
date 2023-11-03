using Konscious.Security.Cryptography;
using System;
using System.Text;

public class PasswordHashService
{
    public string GenerateRandomSalt(int size)
    {
        byte[] salt = new byte[size];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            rng.GetBytes(salt);

        return Convert.ToBase64String(salt);
    }

    public async Task<string> HashPassword(string password, string saltBase64)
    {
        byte[] salt = Convert.FromBase64String(saltBase64);

        using var hasher = new Argon2id(Encoding.UTF8.GetBytes(password));
        hasher.Salt = salt;
        hasher.DegreeOfParallelism = 1;
        hasher.MemorySize = 9216;
        hasher.Iterations = 4;
        var hashedPassword = await hasher.GetBytesAsync(64);

        var hashedPasswordBase64 = Convert.ToBase64String(hashedPassword);
        return hashedPasswordBase64;
    }
}
