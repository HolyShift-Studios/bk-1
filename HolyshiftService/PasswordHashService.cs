using Konscious.Security.Cryptography;
using System;
using System.Text;

public class PasswordHashService
{
    public (string hashedPassword, string salt) HashPassword(string password, string saltBase64 = null)
    {
        byte[] salt;
        if (string.IsNullOrEmpty(saltBase64))
        {
            salt = new byte[16];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                rng.GetBytes(salt);
        }
        else
        {
            salt = Convert.FromBase64String(saltBase64);
        }
        using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
        {
            hasher.Salt = salt;
            hasher.DegreeOfParallelism = 8;
            hasher.MemorySize = 65536;
            hasher.Iterations = 4;

            string hashedPassword = Convert.ToBase64String(hasher.GetBytes(32));
            return (hashedPassword, Convert.ToBase64String(salt));
        }
    }
}
