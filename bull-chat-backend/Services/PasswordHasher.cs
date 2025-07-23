using System.Security.Cryptography;
using System.Text;

namespace bull_chat_backend.Services.Interfaces
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GetHashSHA256(string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = SHA256.HashData(inputBytes);
            return Convert.ToHexString(hashBytes);
        }

        public bool VerifySHA256(string password, string hash)
        {
            var computedHash = GetHashSHA256(password);
            return string.Equals(computedHash, hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}