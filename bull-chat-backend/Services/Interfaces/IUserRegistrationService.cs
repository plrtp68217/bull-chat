using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Services.Interfaces
{
    public interface ILoginResponse
    {
        string Token { get; set; }
        User User { get; set; }
    }

    public class LoginResponse : ILoginResponse
    {
        public string Token { get; set; }
        public User User { get; set; }

        public LoginResponse(string token, User user)
        {
            Token = token;
            User = user;
        }

        public LoginResponse Empty()
        {
            return new LoginResponse("", User.Empty);
        }
    }

    public interface IUserRegistrationService
    {
        Task<ILoginResponse> LoginAsync(string name, string password, CancellationToken token);
        Task<User> RegisterAsync(string name, string password, CancellationToken token);
    }
}