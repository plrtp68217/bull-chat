using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace bull_chat_backend.Hubs
{
    /* Берём userId из JWT claim
     * Оно будет совпадать так как при генерации jwt токена туда закладывается эта информация
     * см. JwtGeneratorService:57
     */
    public class ChatHubUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection) 
            => connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

}
