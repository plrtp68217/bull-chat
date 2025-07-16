namespace bull_chat_backend.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string GetHash(string password);
        bool Verify(string password, string hash);
    }
}
