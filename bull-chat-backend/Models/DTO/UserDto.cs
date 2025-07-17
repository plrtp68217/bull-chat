using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Models.DTO
{
    public class UserDto
    {
        public UserDto(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
