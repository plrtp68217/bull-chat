using bull_chat_backend.Models.DBase;
using System.ComponentModel.DataAnnotations;

namespace bull_chat_backend.Models
{
    public record LoginResponse([Required] string Token, [Required] User User)
    {
        private static readonly LoginResponse _empty = new(string.Empty, User.Empty);
        public static LoginResponse Empty 
        {
            get => _empty;
        }
        
    }
}