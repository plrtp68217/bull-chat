using bull_chat_backend.Models;
using bull_chat_backend.Repository.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace bull_chat_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]/")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly IMessageRepository _messageRepository;
        public record DateRangeRequest(DateTime DateStart, DateTime DateEnd);
        public record LastNFromRequest(int Count, DateTime DateFrom);
        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        
        [HttpPost("n-from-date")]
        public async Task<IActionResult> LastNFromDate([FromBody] LastNFromRequest request)
        {
            var normalizedDate = request.DateFrom.Date;
            var messages = await _messageRepository.GetLastNFromDateAsync(request.Count, normalizedDate);
            return Ok(messages);
        }

        [HttpPost("between-dates")]
        public async Task<IActionResult> BetweenDate([FromBody] DateRangeRequest range)
        {
            var dateStart = range.DateStart.Date;
            var dateTo = range.DateEnd.Date.AddDays(1).AddTicks(-1);

            var messages = await _messageRepository.GetDateIntervalAsync(dateStart, dateTo);
            return Ok(messages);
        }

        [HttpPost("on-date")]
        public async Task<IActionResult> OnDate([FromBody] DateTime date)
        {
            // Отправлять так: new Date().toISOString()
            var dateStart = date.Date;
            var dateTo = dateStart.AddDays(1);

            var messages = await _messageRepository.GetDateIntervalAsync(dateStart, dateTo);
            return Ok(messages);
        }
    }
}
