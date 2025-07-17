using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Services.Interfaces
{
    public interface IUserRegistrationService
    {
        Task<string> LoginAsync(string name, string password, CancellationToken token);
        Task<User> RegisterAsync(string name, string password, CancellationToken token);
    }
}