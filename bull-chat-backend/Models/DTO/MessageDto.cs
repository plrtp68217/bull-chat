namespace bull_chat_backend.Models.DTO
{
    public class MessageDto
    {
        public MessageDto(DateTime date, UserDto? user, ContentDto? content, bool isAuthor = false)
        {
            Date = date;
            User = user;
            Content = content;
            IsAuthor = isAuthor;
        }

        public MessageDto WithAuthorFlag(bool isAuthor)
        {
            return new MessageDto(Date, User, Content, isAuthor);
        }

        public bool IsAuthor { get; }
        public DateTime Date { get; }
        public UserDto? User { get; }
        public ContentDto? Content { get; }
    }
}