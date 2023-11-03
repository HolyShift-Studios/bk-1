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

    public string HashPassword(string password, string saltBase64)
    {
        byte[] salt = Convert.FromBase64String(saltBase64);

        using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
        {
            hasher.Salt = salt;
            hasher.DegreeOfParallelism = 1;
            hasher.MemorySize = 9216;
            hasher.Iterations = 4;

            string hashedPassword = Convert.ToBase64String(hasher.GetBytes(32));
            return hashedPassword;
        }
    }
}
