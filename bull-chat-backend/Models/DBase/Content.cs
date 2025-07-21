using bull_chat_backend.Models.DBase.Enum;
using bull_chat_backend.Models.DTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace bull_chat_backend.Models.DBase
{
    public class Content
    {
        public int Id { get; set; }
        public ContentType ContentType { get; set; }
        public string Item { get; set; }
        public int MessageId { get; set; }
        public Message? Message { get; set; }

        public static bool IsEmpty(Content c) => c.ContentType == Enum.ContentType.Unknown;

        private readonly static Content _empty = new(string.Empty, Enum.ContentType.Unknown);
        // Для EF Core
        public Content() { }
        public Content(string text, ContentType contentType)
        {
            ContentType = contentType;
            Item = text;
        }

        [NotMapped] public static Content Empty => _empty;

        public ContentDto ToDto() => new(Item, ContentType);
    }

}
