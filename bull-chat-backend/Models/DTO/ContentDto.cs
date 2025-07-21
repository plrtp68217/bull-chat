using bull_chat_backend.Models.DBase.Enum;

namespace bull_chat_backend.Models.DTO
{
    public class ContentDto(string item, ContentType content)
    {
        public string Item { get; } = item;
        public ContentType Content { get; } = content;
    }
}
