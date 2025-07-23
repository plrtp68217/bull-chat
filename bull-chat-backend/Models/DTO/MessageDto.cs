namespace bull_chat_backend.Models.DTO
{
    public class MessageDto
    {
        public MessageDto(int id, DateTime date, UserDto? user, ContentDto? content, bool isAuthor = false)
        {
            Date = date;
            User = user;
            Content = content;
            Id = id;
        }
        public int Id { get; set; }
        public DateTime Date { get; }
        public UserDto? User { get; }
        public ContentDto? Content { get; }
    }
}