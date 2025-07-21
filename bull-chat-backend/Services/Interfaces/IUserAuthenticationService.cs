using bull_chat_backend.Models;
using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Services.Interfaces
{
    public interface IUserAuthenticationService
    {
        Task<LoginResponse> LoginAsync(string name, string password, CancellationToken token);
        ValueTask<bool> ValidateTokenAsync(string jwtToken, CancellationToken token);
        Task<User> RegisterAsync(string name, string password, CancellationToken token);
    }
}