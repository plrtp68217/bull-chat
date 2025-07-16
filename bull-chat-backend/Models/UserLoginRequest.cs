using System.ComponentModel.DataAnnotations;

namespace bull_chat_backend.Models
{
    public record UserLoginRequest([Required] string Login, [Required] string Password);
}
