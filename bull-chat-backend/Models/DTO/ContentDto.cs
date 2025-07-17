using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Models.DTO
{
    public class ContentDto
    {
        public int Id { get; set; }

        //Пока не знаю, надо ли?
        //public MessageDto Message { get; set; }
        public string Text { get; set; }
    }
}
