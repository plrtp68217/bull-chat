using bull_chat_backend.Models.DTO;

namespace bull_chat_backend.Models.DBase
{
    public class Message
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public int ContentId { get; set; }

        public User? User { get; set; }
        public Content? Content { get; set; }

        public Message(User msgFrom, string text)
        {
            User = msgFrom;
            Content = new Content()
            {
                Item = text
            };
        }
        public Message() { }
        public static Message Empty => new()
        {
            User = User.Empty,
            Content = Content.Empty,
        };
        //TODO: Может упать если нет User и Content
        public static bool IsEmpty(Message msg) => User.IsEmpty(msg.User!) && Content.IsEmpty(msg.Content!);
    }

    public static class MessageExtensions 
    {
        public static MessageDto ToDto(this Message msg) 
        {
            return new MessageDto()
            {
                Date = msg.Date,
                User = new UserDto()
                {
                    Id = msg.User!.Id,
                    Name = msg.User.Name
                },
                Content = new ContentDto()
                {
                    Item = msg.Content!.Item!
                }
            };
        }
    }
}
