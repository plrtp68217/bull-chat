using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Models.DTO
{
    public class UserDto
    {

        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
