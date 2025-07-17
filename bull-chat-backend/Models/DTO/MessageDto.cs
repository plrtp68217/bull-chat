namespace bull_chat_backend.Models.DTO
{
    public class MessageDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int ContentId { get; set; }

        // Пока так, во избежании рекурсии при маппинге
        public string ContentText { get; set; }
        public string UserName { get; set; }
    }
}
