using bull_chat_backend.Models;
using bull_chat_backend.Models.DBase;
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
        private readonly ChatDbContext _chatDbContext;

        public record DateRangeRequest(DateTime DateStart, DateTime DateEnd);
        public record LastNFromRequest(int Count, DateTime DateFrom);
        public MessageController(IMessageRepository messageRepository, ChatDbContext chatDbContext)
        {
            _messageRepository = messageRepository;
            _chatDbContext = chatDbContext;
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

        [HttpGet("mock")]
        public async Task<IActionResult> Mock(CancellationToken token)
        {
            var contetns = new string[] {
                "Привет, как дела?",
                "Привет! Да нормально, пасемся на этом зеленом лугу. А ты как?",
                "Тоже неплохо! Только вот, кажется, у нас скоро дождь.",
                "О, неужели? Надо будет найти укрытие.",
                "Да, я слышал, что под тем большим деревом всегда сухо.",
                "Отличная идея! А ты слышал, что в соседнем стаде родился теленок?",
                "Да, слышал! Говорят, он очень игривый.",
                "Надо будет сходить его поздравить.",
                "Согласен! А ты уже пробовал ту новую траву, что на восточной стороне луга?",
                "Да, она просто великолепна! Особенно на завтрак.",
                "Я тоже заметил! Надо будет чаще туда заглядывать.",
                "А ты знаешь, что у нас скоро будет праздник?",
                "Какой праздник?",
                "День пастбища! Будем веселиться и есть много травы!",
                "Звучит здорово! Надо подготовиться.",
                "Да, и пригласить всех друзей!",
                "Кстати, ты слышал, кто будет выступать на празднике?",
                "Говорят, что будут лучшие музыканты из соседних стад!",
                "О, это будет весело! Я люблю танцевать под музыку.",
                "Я тоже! Особенно, когда все собираются вместе.",
                "А ты уже выбрал, что будешь носить на праздник?",
                "Думаю, надену свой лучший колокольчик. Он звучит просто замечательно!",
                "Отличный выбор! Я подумаю над чем-то ярким, чтобы выделяться.",
                "Может, украсишь себя цветами? Это всегда выглядит красиво.",
                "Да, это хорошая идея! Надо будет собрать несколько полевых цветов.",
                "А ты знаешь, что на празднике будет конкурс на лучшее украшение?",
                "О, это интересно! Я постараюсь сделать что-то особенное.",
                "Я тоже! Может, мы сможем объединить усилия и сделать что-то вместе?",
                "Звучит здорово! Давай соберем все лучшие идеи.",
                "Да, и будем работать над этим в свободное время.",
                "А когда у нас будет время? Мы же постоянно пасемся!",
                "Ну, можно поработать после обеда, когда все немного отдохнут.",
                "Отлично! Я уже в предвкушении праздника.",
                "Я тоже! Надо будет позвать всех наших друзей.",
                "Да, пусть все приходят! Чем больше, тем веселее.",
                "А ты знаешь, что на празднике будет много вкусной еды?",
                "О, это просто прекрасно! Я люблю вкусную траву.",
                "И я! Надо будет попробовать все, что предложат.",
                "Да, и не забыть про сладкие яблоки!",
                "О, яблоки — это моя слабость! Особенно, если они спелые.",
                "Согласен! Надо будет найти, где их можно достать.",
                "Может, у старого дуба? Там всегда много фруктов.",
                "Отличная идея! Давай заглянем туда после обеда.",
                "Договорились! А ты слышал, что на празднике будет много игр?",
                "Да, я слышал! Будет весело участвовать в конкурсах.",
                "Особенно в тех, где нужно бегать и прыгать!",
                "Да, это всегда поднимает настроение!",
                "А ты знаешь, кто будет судить конкурсы?",
                "Говорят, что старший бык из соседнего стада. Он очень строгий!",
                "О, это будет интересно! Надо будет показать все свои лучшие качества.",
                "Да, и не забыть про уверенность! Главное — не волноваться.",
                "Согласен! Мы же не только для победы, но и для веселья!",
                "Точно! Главное — хорошее настроение.",
                "А ты слышал, что на празднике будет много новых игр?",
                "Да, я слышал! Например, игра в поймай траву ",
                "О, это звучит весело! Как в нее играть?",
                "Нужно будет бегать и пытаться поймать кусочки травы, которые будут разбрасывать.",
                "Звучит просто! Я готов к соревнованию!",
                "Я тоже! Надо будет потренироваться перед праздником.",
                "Да, чтобы быть в форме! Может, устроим тренировку?",
                "Отличная идея! Давай соберем всех друзей и потренируемся вместе.",
                "Супер! Чем больше, тем веселее!",
                "А ты знаешь, что на празднике будет специальный торт из травы?",
                "О, это звучит вкусно! Я никогда не пробовал травяной торт.",
                "Говорят, он очень сладкий и ароматный!",
                "Надо будет обязательно попробовать!",
                "Да, и не забыть сделать фото на память!",
                "Точно! Мы сможем запечатлеть все веселые моменты.",
                "А ты знаешь, что на празднике будет много танцев?",
                "О, я люблю танцевать! Надо будет подготовить несколько движений.",
                "Да, и показать всем, как мы умеем веселиться!",
                "Согласен! Мы станем звездами праздника!",
                "А ты слышал, что будут специальные призы для победителей?",
                "Да, я слышал! Это будет отличная мотивация!",
                "Надо будет постараться, чтобы выиграть что-то интересное.",
                "Да, и поделиться призами с друзьями!",
                "Точно! Главное — это дружба и веселье.",
                "А ты знаешь, что на празднике будет много новых знакомых?",
                "Да, это здорово! Мы сможем завести новых друзей.",
                "И рассказать им о наших приключениях!",
                "Да, и послушать их истории!"
            };

            var currentDate = DateTime.UtcNow.AddDays(-7);
            var msgs = new List<Message>(256);
            var swap = false;
            var user1 = _chatDbContext.User.First(s => s.Id == 1);
            var user2 = _chatDbContext.User.First(s => s.Id == 2);
            _chatDbContext.User.Attach(user1);
            _chatDbContext.User.Attach(user2);

            for (int i = 0; i < 140; i++)
            {
                var text = contetns[i % contetns.Length];
                if (i % 20 == 0)
                {
                    swap = !swap;
                    currentDate.AddDays(1);
                }
                var msg = new Message(swap ? user1 : user2, text) 
                {
                    Date = currentDate,
                };
                msgs.Add(msg);
            }

            await _chatDbContext.Message.AddRangeAsync(msgs);
            await _chatDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
