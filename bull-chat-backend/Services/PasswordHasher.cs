namespace bull_chat_backend.Services.Interfaces
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GetHash(string password) 
            => BCrypt.Net.BCrypt.EnhancedHashPassword(password, hashType: BCrypt.Net.HashType.SHA384);
        public bool Verify(string password, string hash) 
            => BCrypt.Net.BCrypt.EnhancedVerify(password, hash, hashType: BCrypt.Net.HashType.SHA384);
    }
}