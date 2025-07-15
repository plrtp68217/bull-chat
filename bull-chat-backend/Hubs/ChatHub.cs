using bull_chat_backend.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace bull_chat_backend.Hubs
{
    internal interface IChatHub 
    {
        Task SendMessage(string user, string message);
        Task ReceiveMessage(Message message);
        Task NotifyTyping(string user);
        Task GetConnectedUsers();
    }

    internal class ChatHub(ChatDbContext dbContext) : Hub<IChatHub>
    {
        private readonly ChatDbContext _dbContext = dbContext;

        public async Task SendMessage(int userId, string text)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) throw new HubException("User not found");

            Message message = new()
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                User = user,
                Content = new Content {Text = text},
            };

            _dbContext.Message.Add(message);
            await _dbContext.SaveChangesAsync();

            if (message == null)
            {
                throw new HubException("Failed to save message");
            }

            await Clients.All.ReceiveMessage(message);
        }
    }
}
