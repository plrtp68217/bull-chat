namespace bull_chat_backend.Services.Interfaces
{
    public interface IJwtGenerator<in T> where T : class
    {
        string GenerateToken(T user);
    }
}
