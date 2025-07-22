using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Models.DTO
{
    public class UserDto(int id, string name)
    {
        public int Id { get; } = id;
        public string Name { get; } = name;
    }
}
