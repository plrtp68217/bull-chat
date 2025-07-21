namespace bull_chat_backend.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string GetHashSHA256(string password);
        bool VerifySHA256(string password, string hash);
    }
}
