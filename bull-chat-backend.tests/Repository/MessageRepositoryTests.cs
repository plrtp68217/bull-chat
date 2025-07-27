using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository;
using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace bull_chat_backend.Tests.Repository
{
    public class MessageRepositoryTests
    {
        private readonly ChatDbContext _context;
        private readonly IMessageRepository _repository;
        private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;

        public MessageRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ChatDbContext(options);
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _repository = new MessageRepository(_context);
        }

        /// <summary>
        /// Добавляет 10 сообщений с интервалом в 1 минуту.
        /// 
        /// Пример генерации по дате (если baseTime = 10:00):
        /// Message 1  -> 10:01
        /// Message 2  -> 10:02
        /// ...
        /// Message 10 -> 10:10
        /// </summary>
        private async Task Seed10MessagesAsync(DateTime baseTime)
        {
            var user = new User { Id = 1, Name = "TestUser", PasswordHash = "TestUserPassHash" };

            for (int i = 1; i <= 10; i++)
            {
                var msg = new Message
                {
                    Id = i,
                    User = user,
                    Date = baseTime.AddMinutes(i),
                    Content = new Content { Item = $"Message {i}" }
                };

                _context.Message.Add(msg);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Тест метода GetPagedMessages(int cursorIndex, int pageSize)
        /// 
        /// Проверяем, что метод корректно возвращает `pageSize` сообщений, 
        /// которые имеют ID < cursorIndex, отсортированные по возрастанию.
        /// 
        /// cursorIndex = 6, pageSize = 3 → должны вернуться сообщения с ID: 3, 4, 5
        /// 
        /// Графическое представление:
        /// 
        /// Все сообщения:
        /// [1] [2] [3] [4] [5] [6] [7] [8] [9] [10]
        ///              ↑
        ///            cursor = 6
        /// 
        /// Выбираем ID < 6             → [1] [2] [3] [4] [5]
        /// Сортировка по убыванию      → [5] [4] [3] [2] [1]
        /// Take(3)                     → [5] [4] [3]
        /// Сортировка по возрастанию   → [3] [4] [5]
        /// </summary>
        [Fact]
        public async Task GetPagedMessages_ById_ReturnsCorrectPage()
        {
            // Arrange
            var baseTime = DateTime.UtcNow;
            await Seed10MessagesAsync(baseTime);

            int cursorIndex = 6;
            int pageSize = 3;
            var token = CancellationToken.None;

            // Act
            var result = await _repository.GetPagedMessages(cursorIndex, pageSize, token);

            // Assert
            Assert.Equal(pageSize, result.Count);
            Assert.Equal("Message 3", result[0].Content?.Item);
            Assert.Equal("Message 4", result[1].Content?.Item);
            Assert.Equal("Message 5", result[2].Content?.Item);
        }

        /// <summary>
        /// Тест метода GetPagedMessages(DateTime cursorDate, int pageSize)
        /// 
        /// Проверяем, что метод корректно возвращает `pageSize` сообщений, 
        /// у которых Date <= cursorDate, отсортированных по возрастанию даты.
        /// 
        /// Допустим now = 12:00, тогда создаются сообщения от 11:41 (Message 1) до 11:50 (Message 10)
        /// cursorDate = 11:55 → попадут все сообщения
        /// 
        /// Отбираем последние 4 по дате:
        /// 11:50 - [Message 10]  
        /// 11:49 - [Message 09] 
        /// 11:48 - [Message 08] 
        /// 11:47 - [Message 07] 
        /// 
        /// Возвращаем в хронологическом порядке:
        /// [Message 07] → 11:47
        /// [Message 08] → 11:48
        /// [Message 09] → 11:49
        /// [Message 10] → 11:50
        /// 
        /// Графическое представление:
        /// 
        /// Timeline:           now = 12:00
        /// [M1][M2]...[M6][M7][M8][M9][M10]
        ///                   ↑
        ///               cursorDate = 11:55
        /// 
        /// Выбираем последние 4 до этой точки, по дате → [M10, M9, M8, M7] → пересортировано → [M7–M10]
        /// </summary>
        [Fact]
        public async Task GetPagedMessages_ByDate_ReturnsCorrectPage()
        {
            // Arrange
            var now = new DateTime(
                year: 2025,
                month: 1,
                day: 1,
                hour: 12,
                minute: 0,
                second: 0);
            _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);

            await Seed10MessagesAsync(now.AddMinutes(-20)); // c 11:41 до 11:50

            int pageSize = 4;
            var token = CancellationToken.None;
            DateTime cursorDate = now.AddMinutes(-5); // 11:55

            // Act
            var result = await _repository.GetPagedMessages(cursorDate, pageSize, token);

            // Assert
            Assert.Equal(pageSize, result.Count);
            Assert.True(result.All(m => m.Date <= cursorDate));
            Assert.Equal("Message 7", result[0].Content?.Item); // 11:47
            Assert.Equal("Message 10", result[3].Content?.Item); // 11:50
        }

        [Fact]
        public async Task GetPagedMessages_ByDate_ExcludesMessagesEqualToCursorDate()
        {
            // Arrange
            var now = new DateTime(2025, 1, 1, 12, 0, 0); // Текущее "now" — 12:00
            _mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);

            /*
                Генерация сообщений:
                Message 01 → 11:46
                Message 02 → 11:47
                Message 03 → 11:48
                Message 04 → 11:49
                Message 05 → 11:50
                Message 06 → 11:51
                Message 07 → 11:52 ← cursorDate
                Message 08 → 11:53 
                Message 09 → 11:54
                Message 10 → 11:55 
            */
            await Seed10MessagesAsync(now.AddMinutes(-15)); // от 11:46 до 11:55

            int pageSize = 2;
            var token = CancellationToken.None;
            DateTime cursorDate = now.AddMinutes(-8); // cursorDate = 11:52

            // Act
            var result = await _repository.GetPagedMessages(cursorDate, pageSize, token);

            // Assert
            Assert.Equal(pageSize, result.Count);
            Assert.All(result, msg => Assert.True(msg.Date < cursorDate));

            var expectedMessages = new[] {
                "Message 5", // 11:50
                "Message 6", // 11:51
            };

            Assert.Equal(expectedMessages, result.Select(m => m.Content?.Item));
        }

    }
}
