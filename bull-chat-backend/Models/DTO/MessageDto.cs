using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Models.DTO
{
    public class MessageDto
    {
        public MessageDto() { }
        public MessageDto(DateTime date, UserDto? user, ContentDto? content)
        {
            Date = date;
            User = user;
            Content = content;
        }

        public DateTime Date { get; set; }
        public UserDto? User { get; set; }
        public ContentDto? Content { get; set; }

    }
}
