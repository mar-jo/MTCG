using System.Security.Cryptography;

namespace MTCG.Database.Hashing;

public class PasswordHasher
{
    public (string, string) Hash(string password)
    {
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
    
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);
    
        string hashString = Convert.ToBase64String(hash);
        string saltString = Convert.ToBase64String(salt);
    
        return (hashString, saltString);
    }

    public bool Verify(string password, string? hashString, string? saltString)
    {
        if (password == null || hashString == null || saltString == null)
            return false;

        byte[] hash = Convert.FromBase64String(hashString);
        byte[] salt = Convert.FromBase64String(saltString);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] rehash = pbkdf2.GetBytes(20);

        if (hash.Length != rehash.Length) return false;
        for (int i = 0; i < hash.Length; i++)
        {
            if (hash[i] != rehash[i]) return false;
        }

        return true;
    }
}