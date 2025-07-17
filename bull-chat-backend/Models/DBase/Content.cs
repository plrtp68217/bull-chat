using bull_chat_backend.Models.DBase.Enum;

namespace bull_chat_backend.Models.DBase
{
    public class Content
    {
        public int Id { get; set; }
        public ContentType? ContentType { get; set; }
        public string? Item { get; set; }

        public static bool IsEmpty(Content c) => c.ContentType == Enum.ContentType.Unknown;
        public static Content Empty => new()
        {
            ContentType = Enum.ContentType.Unknown,
            Item = string.Empty
        };
    }

}
