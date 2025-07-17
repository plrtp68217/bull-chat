using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Models.DTO
{
    public class ContentDto
    {
        public string Item { get; set; }

        public ContentDto(string item)
        {
            Item = item;
        }
    }
}
