using bull_chat_backend.Hubs;
using bull_chat_backend.ModelBindings.Attributes;
using bull_chat_backend.Models;
using bull_chat_backend.Models.DBase;
using bull_chat_backend.Models.DBase.Enum;
using bull_chat_backend.Repository.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace bull_chat_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]/")]
    [ApiController]
    public class MessageController(IMessageRepository messageRepository) : Controller
    {
        private readonly IMessageRepository _messageRepository = messageRepository;

        public record DateRangeRequest(DateTime DateStart, DateTime DateEnd);
        public record LastNFromRequest(int Count, DateTime DateFrom);

        [HttpPost("last-message-date")]
        public async Task<IActionResult> LastMessageDate(CancellationToken token)
        {
            var messages = await _messageRepository.LastMessage(token);
            return Ok(messages);
        }

        [HttpPost("next-message-page-index")]
        public async Task<IActionResult> NextMessagePage(
            [FromBody] int? messageId,
                       CancellationToken token)
        {
            if (!messageId.HasValue) 
            {
                var lastMessage = await _messageRepository.LastMessage(token);
                messageId = lastMessage.Id + 1;
            }

            const int PAGE_SIZE = 100;
            var messages = await _messageRepository.GetPagedMessages(messageId.Value, PAGE_SIZE, token);
            return Ok(messages);
        }
        [HttpPost("next-message-page-date")]
        public async Task<IActionResult> NextMessagePageDate([FromBody] DateTime? messageDate, CancellationToken token)
        {
            if (!messageDate.HasValue)
            {
                var lastMessage = await _messageRepository.LastMessage(token);
                messageDate = lastMessage.Date;
            }

            const int PAGE_SIZE = 100;
            var messages = await _messageRepository.GetPagedMessages(messageDate.Value, PAGE_SIZE, token);
            return Ok(messages);
        }

        // Костыл, СВАГА не понимает IFormFile, но зато обертку понимает (походе дебил)
        public record UploadImageRequest(IFormFile File);
        [HttpPost("media/upload-image")]
        public async Task<IActionResult> UploadImage(
            [UserFromRequest] User user,
            [FromForm]        UploadImageRequest file,
            [FromServices]    IOptions<ImageStorageOptions> storageOptions,
            [FromServices]    IHubContext<ChatHub, IChatHub> hubContext,
                              CancellationToken token)
        {
            var options = storageOptions.Value;

            if (file == null || file.File.Length == 0)
                return BadRequest("Файл не выбран");

            if (file.File.Length > options.MaxImageSizeBytes)
                return BadRequest("Размер изображения превышает допустимый лимит.");

            var fileExtension = Path.GetExtension(file.File.FileName).ToLowerInvariant();
            if (!options.AllowedImageExtensions.Contains(fileExtension))
                return BadRequest("Недопустимый формат изображения.");

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), options.ImageFolder);
            Directory.CreateDirectory(savePath);
            var filePath = Path.Combine(savePath, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.File.CopyToAsync(stream, token);

            var relativePath = $"{options.ImageRequestPath}/{fileName}".Replace('\\', '/');

            var message = new Message(user, DateTime.UtcNow, ContentType.Image, relativePath);
            await _messageRepository.AddAsync(message, token);

            await hubContext.Clients.All.ReceiveMessage(message.ToDto());

            return Ok(message.ToDto());
        }



    }
}
