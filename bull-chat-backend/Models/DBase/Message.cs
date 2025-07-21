using bull_chat_backend.Models.DBase.Enum;
using bull_chat_backend.Models.DTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace bull_chat_backend.Models.DBase
{
    public class Message
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }

        public User User { get; set; }
        public Content Content { get; set; }

        public MessageDto ToDto() => new MessageDto(Date, User.ToDto(), Content.ToDto());
        [NotMapped] public static Message Empty => new(User.Empty, string.Empty);
        public static bool IsEmpty(Message msg) => User.IsEmpty(msg.User!) && Content.IsEmpty(msg.Content!);
        public Message(User msgFrom, string text)
        {
            //Пока так, только текст.
            User = msgFrom;
            Content = new Content(text, ContentType.Text);
        }
        // Для EF Core
        public Message() { }
    }
}
