namespace bull_chat_backend.Services.Interfaces
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GetHashSHA256(string password)
            => BCrypt.Net.BCrypt.EnhancedHashPassword(password, hashType: BCrypt.Net.HashType.SHA256);
        public bool VerifySHA256(string password, string hash)
            => BCrypt.Net.BCrypt.EnhancedVerify(password, hash, hashType: BCrypt.Net.HashType.SHA256);
    }
}