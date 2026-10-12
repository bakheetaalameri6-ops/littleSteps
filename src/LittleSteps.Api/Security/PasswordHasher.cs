using System.Security.Cryptography;
using LittleSteps.Api.Domain;
namespace LittleSteps.Api.Security;
public static class PasswordHasher
{
    public static (byte[] Hash,byte[] Salt) Create(string password) {Rules.Password(password);var salt=RandomNumberGenerator.GetBytes(16);return(Derive(password,salt),salt);}
    private static byte[] Derive(string password,byte[] salt)=>Rfc2898DeriveBytes.Pbkdf2(password,salt,100_000,HashAlgorithmName.SHA256,32);
    public static bool Verify(string password,byte[] hash,byte[] salt)=>CryptographicOperations.FixedTimeEquals(Derive(password,salt),hash);
}
