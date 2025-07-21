namespace bull_chat_backend.Services.Interfaces
{
    public interface IJwtGenerator<in T> where T : class
    {
        ValueTask<bool> ValidateTokenAsync(string token);
        string GenerateToken(T user);
    }
}
