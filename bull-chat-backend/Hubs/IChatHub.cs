using bull_chat_backend.Models.DTO;

namespace bull_chat_backend.Hubs
{
    public interface IChatHub 
    {
        Task SendMessage(int userId, string content);
        Task ReceiveMessage(MessageDto messageDto);
        //Task NotifyTyping(string user);
        //Task GetConnectedUsers();
    }
}
