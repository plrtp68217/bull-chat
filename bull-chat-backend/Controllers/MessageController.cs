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


        [HttpPost("last-n-from-date")]
        public async Task<IActionResult> LastNFromDate([FromBody] LastNFromRequest request, CancellationToken token)
        {
            var normalizedDate = request.DateFrom.Date;
            var messages = await _messageRepository.GetLastNFromDateAsync(request.Count, normalizedDate, token);
            return Ok(messages);
        }

        [HttpPost("between-dates")]
        public async Task<IActionResult> BetweenDate([FromBody] DateRangeRequest range, CancellationToken token)
        {
            var dateStart = range.DateStart.Date;
            var dateTo = range.DateEnd.Date.AddDays(1).AddTicks(-1);

            var messages = await _messageRepository.GetDateIntervalAsync(dateStart, dateTo, token);
            return Ok(messages);
        }

        [HttpPost("on-date")]
        public async Task<IActionResult> OnDate([FromBody] DateTime date, CancellationToken token)
        {
            var dateStart = date.Date;
            var dateTo = dateStart.AddDays(1);

            var messages = await _messageRepository.GetDateIntervalAsync(dateStart, dateTo, token);
            return Ok(messages);
        }

        [HttpPost("last-message-date")]
        public async Task<IActionResult> LastMessageDate(CancellationToken token)
        {
            var messages = await _messageRepository.LastMessageDate(token);
            return Ok(messages);
        }
    }
}
